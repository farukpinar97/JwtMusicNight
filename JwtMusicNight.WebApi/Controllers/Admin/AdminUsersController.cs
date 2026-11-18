using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Entities;
using JwtMusicNight.WebApi.Services.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers.Admin
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = AdminSeeder.AdminRole)]
    public class AdminUsersController : ControllerBase
    {
        private readonly IAdminUserService _service;
        public AdminUsersController(IAdminUserService service) => _service = service;

        // GET api/admin/users?search=gold&package=Gold
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] PackageLevel? package)
            => Ok(await _service.GetUsersAsync(search, package));

        // PUT api/admin/users/{id}/package   { "packageLevel": "Premium" }
        [HttpPut("{id}/package")]
        public async Task<IActionResult> UpdatePackage(string id, UpdatePackageDto dto)
        {
            var ok = await _service.UpdatePackageAsync(id, dto.PackageLevel);
            if (!ok) return NotFound(new { message = "Kullanıcı bulunamadı." });

            return Ok(new
            {
                message = $"Paket {dto.PackageLevel} olarak güncellendi. " +
                          "Yeni paket, kullanıcı tekrar giriş yapıp yeni token aldığında geçerli olur."
            });
        }
    }
}
