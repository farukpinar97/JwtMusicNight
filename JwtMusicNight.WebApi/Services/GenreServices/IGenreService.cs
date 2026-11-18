using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.GenreServices
{
    public interface IGenreService
    {
        Task<List<ResultGenreDto>> GetAllGenresAsync();
        Task<ResultGenreDto?> GetGenreByIdAsync(int id);
    }
}
