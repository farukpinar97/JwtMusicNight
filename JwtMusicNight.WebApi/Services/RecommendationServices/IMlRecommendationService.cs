using JwtMusicNight.WebApi.Dtos;

namespace JwtMusicNight.WebApi.Services.RecommendationServices
{
    public interface IMlRecommendationService
    {
        Task<ForYouResultDto> RecommendForUserAsync(string userId, int count = 6);
    }
}
