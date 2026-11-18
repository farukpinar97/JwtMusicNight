using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Services.ArtistServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]   // -> api/artists
    [ApiController]
    [Authorize]                   // Giriş yapmış tüm kullanıcılar
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistService _artistService;

        public ArtistsController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        // GET api/artists
        [HttpGet]
        public async Task<IActionResult> ArtistList()
            => Ok(await _artistService.GetAllArtistsAsync());

        // GET api/artists/3 -> sanatçı detayı + şarkıları
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetArtist(int id)
        {
            var artist = await _artistService.GetArtistByIdAsync(id);
            return artist == null ? NotFound(new { message = "Sanatçı bulunamadı." }) : Ok(artist);
        }

        // POST api/artists
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateArtist(CreateArtistDto createArtistDto)
        {
            await _artistService.CreateArtistAsync(createArtistDto);
            return Ok(new { message = "Sanatçı eklendi." });
        }
    }
}
