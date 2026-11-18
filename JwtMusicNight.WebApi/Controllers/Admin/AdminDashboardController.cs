using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Services.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers.Admin
{
    [Route("api/admin/dashboard")]
    [ApiController]
    [Authorize(Roles = AdminSeeder.AdminRole)]   // JWT içinde "role: Admin" yoksa -> 403
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminUserService _service;
        public AdminDashboardController(IAdminUserService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _service.GetDashboardAsync());
    }
}
