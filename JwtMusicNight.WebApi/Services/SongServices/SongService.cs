using AutoMapper;
using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtMusicNight.WebApi.Services.SongServices
{
    public class SongService : ISongService
    {
        private readonly JwtContext _context;
        private readonly IMapper _mapper;

        public SongService(JwtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Sanatçı ve tür bilgisiyle birlikte sorgu
        private IQueryable<Song> SongsWithDetails() =>
            _context.Songs
                .AsNoTracking()
                .Include(s => s.Artists)
                .Include(s => s.Genre);

        public async Task<List<ResultSongDto>> GetAllSongsAsync()
        {
            var songs = await SongsWithDetails()
                .OrderBy(s => s.RequiredPackage)
                .ThenBy(s => s.SongName)
                .ToListAsync();
            return _mapper.Map<List<ResultSongDto>>(songs);
        }

        public async Task<ResultSongDto?> GetSongByIdAsync(int id)
        {
            var song = await SongsWithDetails().FirstOrDefaultAsync(s => s.SongId == id);
            return song == null ? null : _mapper.Map<ResultSongDto>(song);
        }

        public async Task<List<ResultSongDto>> GetSongsByArtistAsync(int artistId)
        {
            var songs = await SongsWithDetails()
                .Where(s => s.ArtistId == artistId)
                .OrderBy(s => s.RequiredPackage)
                .ToListAsync();
            return _mapper.Map<List<ResultSongDto>>(songs);
        }

        public async Task<List<ResultSongDto>> GetSongsByGenreAsync(int genreId)
        {
            var songs = await SongsWithDetails()
                .Where(s => s.GenreId == genreId)
                .OrderBy(s => s.RequiredPackage)
                .ToListAsync();
            return _mapper.Map<List<ResultSongDto>>(songs);
        }

        public async Task<Song?> GetSongEntityAsync(int id)
        {
            return await _context.Songs.AsNoTracking().FirstOrDefaultAsync(s => s.SongId == id);
        }

        public async Task AddListenAsync(int songId, string userId)
        {
            _context.ListenHistories.Add(new ListenHistory
            {
                SongId = songId,
                AppUserId = userId,
                ListenedAt = DateTime.Now
            });

            // ListenCount'u tek bir UPDATE ile artır (EF Core 7+)
            await _context.Songs
                .Where(s => s.SongId == songId)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.ListenCount, x => x.ListenCount + 1));

            await _context.SaveChangesAsync();
        }
            public async Task<List<ResultSongDto>> GetRecommendationsAsync(int songId, string? userId, int count = 5)
        {
            var currentSong = await _context.Songs.AsNoTracking().FirstOrDefaultAsync(s => s.SongId == songId);
            if (currentSong == null) return new List<ResultSongDto>();

            // 1) Bu şarkıyı dinlemiş DİĞER kullanıcılar
            var listenerIds = _context.ListenHistories
                .Where(h => h.SongId == songId && h.AppUserId != userId)
                .Select(h => h.AppUserId)
                .Distinct();

            // 2) Bu kullanıcıların dinlediği diğer şarkılar (dinleyici sayısına göre skor)
            var recommendedIds = await _context.ListenHistories
                .Where(h => listenerIds.Contains(h.AppUserId) && h.SongId != songId)
                .GroupBy(h => h.SongId)
                .Select(g => new { SongId = g.Key, Score = g.Select(x => x.AppUserId).Distinct().Count() })
                .OrderByDescending(x => x.Score)
                .Take(count)
                .Select(x => x.SongId)
                .ToListAsync();

            var songs = await SongsWithDetails()
                .Where(s => recommendedIds.Contains(s.SongId))
                .ToListAsync();

            // Skor sırasını koru
            var result = songs.OrderBy(s => recommendedIds.IndexOf(s.SongId)).ToList();

            // 3) Yeterli geçmiş yoksa: aynı tür / aynı sanatçıdan en çok dinlenenlerle tamamla
            if (result.Count < count)
            {
                var excludeIds = result.Select(s => s.SongId).Append(songId).ToList();

                var fallback = await SongsWithDetails()
                    .Where(s => !excludeIds.Contains(s.SongId) &&
                                (s.GenreId == currentSong.GenreId || s.ArtistId == currentSong.ArtistId))
                    .OrderByDescending(s => s.ListenCount)
                    .Take(count - result.Count)
                    .ToListAsync();

                result.AddRange(fallback);
            }

            return _mapper.Map<List<ResultSongDto>>(result);
        }

        public async Task<List<ResultSongDto>> GetUserHistoryAsync(string userId, int count = 10)
        {
            // 1) Kullanıcının son dinlediği şarkıların Id'leri (en yeni önce, tekrarsız)
            var songIds = await _context.ListenHistories
                .AsNoTracking()
                .Where(h => h.AppUserId == userId)
                .GroupBy(h => h.SongId)
                .Select(g => new { SongId = g.Key, LastListened = g.Max(x => x.ListenedAt) })
                .OrderByDescending(x => x.LastListened)
                .Take(count)
                .Select(x => x.SongId)
                .ToListAsync();

            // 2) Şarkıları sanatçı ve tür bilgisiyle çek
            var songs = await SongsWithDetails()
                .Where(s => songIds.Contains(s.SongId))
                .ToListAsync();

            // 3) Dinlenme sırasını koru
            var ordered = songs.OrderBy(s => songIds.IndexOf(s.SongId)).ToList();
            return _mapper.Map<List<ResultSongDto>>(ordered);
        }

        public async Task<List<ResultSongDto>> GetChartsAsync(int? genreId, string sort, int count)
        {
            var query = SongsWithDetails();

            if (genreId.HasValue)
                query = query.Where(s => s.GenreId == genreId.Value);

            query = sort == "new"
                ? query.OrderByDescending(s => s.ReleaseDate).ThenByDescending(s => s.ListenCount)
                : query.OrderByDescending(s => s.ListenCount).ThenBy(s => s.SongName);

            var songs = await query.Take(count).ToListAsync();
            return _mapper.Map<List<ResultSongDto>>(songs);
        }
    }
}
