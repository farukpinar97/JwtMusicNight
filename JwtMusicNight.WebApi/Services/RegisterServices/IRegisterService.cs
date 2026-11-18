using JwtMusicNight.WebApi.Dtos;
using Microsoft.AspNetCore.Identity;

namespace JwtMusicNight.WebApi.Services.RegisterServices
{
    public interface IRegisterService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
    }
}