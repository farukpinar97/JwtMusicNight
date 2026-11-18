using JwtMusicNight.WebUI.Dtos;
using JwtMusicNight.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebUI.Controllers
{
    public class GenreController : MusicBaseController
    {
        public GenreController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        // Tüm türler
        public async Task<IActionResult> Index()
        {
            var genres = await GetAsync<List<ResultGenreDto>>("api/genres") ?? new();
            return View(genres);
        }

        // Türe ait şarkılar
        public async Task<IActionResult> Detail(int id)
        {
            var genre = await GetAsync<ResultGenreDto>($"api/genres/{id}");
            if (genre == null) return NotFound();

            var model = new GenreDetailViewModel
            {
                Genre = genre,
                Songs = await GetAsync<List<ResultSongDto>>($"api/songs/genre/{id}") ?? new(),
                AllGenres = await GetAsync<List<ResultGenreDto>>("api/genres") ?? new()
            };
            return View(model);
        }
    }
}
