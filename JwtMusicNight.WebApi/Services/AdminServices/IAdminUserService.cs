using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;

namespace JwtMusicNight.WebApi.Services.AdminServices
{
    public interface IAdminUserService
    {
        Task<AdminDashboardDto> GetDashboardAsync();
        Task<List<AdminUserDto>> GetUsersAsync(string? search, PackageLevel? package);
        Task<bool> UpdatePackageAsync(string userId, PackageLevel package);
    }
}
