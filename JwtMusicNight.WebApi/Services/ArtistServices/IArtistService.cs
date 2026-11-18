using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.ArtistServices
{
    public interface IArtistService
    {
        Task<List<ResultArtistDto>> GetAllArtistsAsync();
        Task<ResultArtistDetailDto?> GetArtistByIdAsync(int id);
        Task CreateArtistAsync(CreateArtistDto createArtistDto);
    }
}
