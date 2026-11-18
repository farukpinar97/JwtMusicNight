using JwtMusicNight.WebApi.Services.GenreServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]   // -> api/genres
    [ApiController]
    [Authorize]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        // GET api/genres
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _genreService.GetAllGenresAsync());

        // GET api/genres/2
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            return genre == null ? NotFound(new { message = "Tür bulunamadı." }) : Ok(genre);
        }
    }
}
