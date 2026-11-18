using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Services.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers.Admin
{
    [Route("api/admin/artists")]
    [ApiController]
    [Authorize(Roles = AdminSeeder.AdminRole)]
    public class AdminArtistsController : ControllerBase
    {
        private readonly IAdminCatalogService _service;
        public AdminArtistsController(IAdminCatalogService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetArtistsAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var artist = await _service.GetArtistAsync(id);
            return artist == null ? NotFound(new { message = "Sanatçı bulunamadı." }) : Ok(artist);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminArtistDto dto)
            => Ok(new { artistId = await _service.CreateArtistAsync(dto), message = "Sanatçı eklendi." });

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, AdminArtistDto dto)
            => await _service.UpdateArtistAsync(id, dto)
                ? Ok(new { message = "Sanatçı güncellendi." })
                : NotFound(new { message = "Sanatçı bulunamadı." });

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, error) = await _service.DeleteArtistAsync(id);
            return ok ? Ok(new { message = "Sanatçı silindi." }) : BadRequest(new { message = error });
        }
    }
}
