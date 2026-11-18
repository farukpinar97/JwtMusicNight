using JwtMusicNight.WebUI.Dtos;
using JwtMusicNight.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebUI.Controllers
{
    public class ChartController : MusicBaseController
    {
        public ChartController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        // /Chart?genreId=5&sort=new
        public async Task<IActionResult> Index(int? genreId, string sort = "popular")
        {
            sort = sort == "new" ? "new" : "popular";
            var url = $"api/songs/charts?sort={sort}&count=20" + (genreId.HasValue ? $"&genreId={genreId}" : "");

            var model = new ChartViewModel
            {
                Songs = await GetAsync<List<ResultSongDto>>(url) ?? new(),
                Genres = await GetAsync<List<ResultGenreDto>>("api/genres") ?? new(),
                SelectedGenreId = genreId,
                Sort = sort
            };
            return View(model);
        }
    }
}
