using JwtMusicNight.WebUI.Dtos;
using JwtMusicNight.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebUI.Controllers
{
    // Hesabım: paket bilgisi, kilitli şarkılar, dinleme geçmişi
    public class AccountController : MusicBaseController
    {
        public AccountController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<IActionResult> Index()
        {
            var model = new AccountViewModel
            {
                Songs = await GetAsync<List<ResultSongDto>>("api/songs") ?? new(),
                History = await GetAsync<List<ResultSongDto>>("api/songs/history") ?? new()
            };
            return View(model);
        }
    }
}
