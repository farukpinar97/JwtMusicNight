using AutoMapper;
using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace JwtMusicNight.WebApi.Services.RecommendationServices
{
    /*
     * ML.NET One-Class Matrix Factorization ile öneri.
     * Model, dinleme geçmişi değiştiğinde yeniden eğitilir.
     */
    public class MlRecommendationService : IMlRecommendationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MlRecommendationService> _logger;
        private readonly MLContext _mlContext = new(seed: 1);
        private readonly SemaphoreSlim _lock = new(1, 1);

        private ITransformer? _model;
        private int _trainedHistoryCount = -1;

        public MlRecommendationService(IServiceScopeFactory scopeFactory, ILogger<MlRecommendationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<ForYouResultDto> RecommendForUserAsync(string userId, int count = 6)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<JwtContext>();
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

            await EnsureModelAsync(context);

            var listenedIds = await context.ListenHistories
                .Where(h => h.AppUserId == userId)
                .Select(h => h.SongId)
                .Distinct()
                .ToListAsync();

            var candidateIds = await context.Songs
                .Where(s => !listenedIds.Contains(s.SongId))
                .Select(s => s.SongId)
                .ToListAsync();

            var recommendedIds = new List<int>();

            if (_model != null && listenedIds.Any() && candidateIds.Any())
            {
                // Aday şarkıların skorlarını toplu tahmin et
                var input = candidateIds.Select(id => new ListenData { UserId = userId, SongId = id.ToString() });
                var predictions = _model.Transform(_mlContext.Data.LoadFromEnumerable(input));

                recommendedIds = _mlContext.Data
                    .CreateEnumerable<ListenPrediction>(predictions, reuseRowObject: false)
                    .Zip(candidateIds, (p, id) => new { id, p.Score })
                    .Where(x => !float.IsNaN(x.Score))          // modelin hiç görmediği şarkı/kullanıcı -> NaN
                    .OrderByDescending(x => x.Score)
                    .Take(count)
                    .Select(x => x.id)
                    .ToList();
            }

            var source = "ML.NET";

            // Geçmiş yoksa veya tahmin yetersizse: dinlenmemiş en popülerler
            if (recommendedIds.Count < count)
            {
                if (recommendedIds.Count == 0) source = "Popular";

                var fill = await context.Songs
                    .Where(s => !listenedIds.Contains(s.SongId) && !recommendedIds.Contains(s.SongId))
                    .OrderByDescending(s => s.ListenCount)
                    .Take(count - recommendedIds.Count)
                    .Select(s => s.SongId)
                    .ToListAsync();
                recommendedIds.AddRange(fill);
            }

            var songs = await context.Songs
                .AsNoTracking()
                .Include(s => s.Artists)
                .Include(s => s.Genre)
                .Where(s => recommendedIds.Contains(s.SongId))
                .ToListAsync();

            return new ForYouResultDto
            {
                Source = source,
                Songs = mapper.Map<List<ResultSongDto>>(songs.OrderBy(s => recommendedIds.IndexOf(s.SongId)))
            };
        }

        // Dinleme geçmişi değiştiyse modeli yeniden eğitir
        private async Task EnsureModelAsync(JwtContext context)
        {
            var historyCount = await context.ListenHistories.CountAsync();
            if (_model != null && historyCount == _trainedHistoryCount) return;

            await _lock.WaitAsync();
            try
            {
                if (_model != null && historyCount == _trainedHistoryCount) return;

                // Her (kullanıcı, şarkı) çifti tek satır
                var data = await context.ListenHistories
                    .GroupBy(h => new { h.AppUserId, h.SongId })
                    .Select(g => new { g.Key.AppUserId, g.Key.SongId })
                    .ToListAsync();

                if (data.Count < 5)
                {
                    _model = null;            // çok az veri -> popüler şarkılar
                    _trainedHistoryCount = historyCount;
                    return;
                }

                var trainingData = _mlContext.Data.LoadFromEnumerable(
                    data.Select(d => new ListenData { UserId = d.AppUserId, SongId = d.SongId.ToString(), Label = 1f }));

                var options = new MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "UserKey",
                    MatrixRowIndexColumnName = "SongKey",
                    LabelColumnName = nameof(ListenData.Label),
                    LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                    Alpha = 0.01,
                    Lambda = 0.025,
                    C = 0.00001,
                    ApproximationRank = 16,
                    NumberOfIterations = 50,
                    Quiet = true
                };

                var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("UserKey", nameof(ListenData.UserId))
                    .Append(_mlContext.Transforms.Conversion.MapValueToKey("SongKey", nameof(ListenData.SongId)))
                    .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(options));

                _model = pipeline.Fit(trainingData);
                _trainedHistoryCount = historyCount;

                _logger.LogInformation("ML.NET öneri modeli eğitildi. Kayıt: {Count}", data.Count);
            }
            finally
            {
                _lock.Release();
            }
        }

        private class ListenData
        {
            public string UserId { get; set; } = "";
            public string SongId { get; set; } = "";
            public float Label { get; set; }
        }

        private class ListenPrediction
        {
            public float Score { get; set; }
        }
    }
}
