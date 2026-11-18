using JwtMusicNight.WebUI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebUI.Controllers
{
    public class ArtistController : MusicBaseController
    {
        public ArtistController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        // Tüm sanatçılar
        public async Task<IActionResult> Index()
        {
            var artists = await GetAsync<List<ResultArtistDto>>("api/artists") ?? new();
            return View(artists);
        }

        // Sanatçı detayı + tüm şarkıları
        public async Task<IActionResult> Detail(int id)
        {
            var artist = await GetAsync<ResultArtistDetailDto>($"api/artists/{id}");
            if (artist == null) return NotFound();
            return View(artist);
        }
    }
}
