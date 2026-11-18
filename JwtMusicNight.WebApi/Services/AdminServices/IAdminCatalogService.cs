using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.AdminServices
{
    // Şarkı / Sanatçı / Tür yönetimi (CRUD)
    public interface IAdminCatalogService
    {
        // Songs
        Task<List<AdminSongDto>> GetSongsAsync(string? search);
        Task<AdminSongDto?> GetSongAsync(int id);
        Task<int> CreateSongAsync(AdminSongDto dto);
        Task<bool> UpdateSongAsync(int id, AdminSongDto dto);
        Task<string?> DeleteSongAsync(int id);              // silinen şarkının MP3 dosya adı
        Task<bool> SetAudioFileAsync(int id, string fileName);

        // Artists
        Task<List<AdminArtistDto>> GetArtistsAsync();
        Task<AdminArtistDto?> GetArtistAsync(int id);
        Task<int> CreateArtistAsync(AdminArtistDto dto);
        Task<bool> UpdateArtistAsync(int id, AdminArtistDto dto);
        Task<(bool ok, string? error)> DeleteArtistAsync(int id);

        // Genres
        Task<List<AdminGenreDto>> GetGenresAsync();
        Task<AdminGenreDto?> GetGenreAsync(int id);
        Task<int> CreateGenreAsync(AdminGenreDto dto);
        Task<bool> UpdateGenreAsync(int id, AdminGenreDto dto);
        Task<(bool ok, string? error)> DeleteGenreAsync(int id);
    }
}
