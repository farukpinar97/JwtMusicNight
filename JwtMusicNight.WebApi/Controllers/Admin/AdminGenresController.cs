using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Services.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers.Admin
{
    [Route("api/admin/genres")]
    [ApiController]
    [Authorize(Roles = AdminSeeder.AdminRole)]
    public class AdminGenresController : ControllerBase
    {
        private readonly IAdminCatalogService _service;
        public AdminGenresController(IAdminCatalogService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetGenresAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var genre = await _service.GetGenreAsync(id);
            return genre == null ? NotFound(new { message = "Tür bulunamadı." }) : Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminGenreDto dto)
            => Ok(new { genreId = await _service.CreateGenreAsync(dto), message = "Tür eklendi." });

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, AdminGenreDto dto)
            => await _service.UpdateGenreAsync(id, dto)
                ? Ok(new { message = "Tür güncellendi." })
                : NotFound(new { message = "Tür bulunamadı." });

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, error) = await _service.DeleteGenreAsync(id);
            return ok ? Ok(new { message = "Tür silindi." }) : BadRequest(new { message = error });
        }
    }
}
