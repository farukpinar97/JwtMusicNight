using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;

namespace JwtMusicNight.WebApi.Services.SongServices
{
    public interface ISongService
    {
        Task<List<ResultSongDto>> GetAllSongsAsync();
        Task<ResultSongDto?> GetSongByIdAsync(int id);
        Task<List<ResultSongDto>> GetSongsByArtistAsync(int artistId);
        Task<List<ResultSongDto>> GetSongsByGenreAsync(int genreId);

        // Stream için entity'nin kendisi lazım (AudioUrl + RequiredPackage)
        Task<Song?> GetSongEntityAsync(int id);

        // Dinleme geçmişine kayıt + ListenCount artırma
        Task AddListenAsync(int songId, string userId);

        // "Bu şarkıyı dinleyenler bunları da dinledi"
        Task<List<ResultSongDto>> GetRecommendationsAsync(int songId, string? userId, int count = 5);

        // Kullanıcının son dinledikleri
        Task<List<ResultSongDto>> GetUserHistoryAsync(string userId, int count = 10);

        Task<List<ResultSongDto>> GetChartsAsync(int? genreId, string sort, int count);
    }
}
