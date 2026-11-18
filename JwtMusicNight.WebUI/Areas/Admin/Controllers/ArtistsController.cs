using JwtMusicNight.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebUI.Areas.Admin.Controllers
{
    public class ArtistsController : AdminBaseController
    {
        public ArtistsController(IHttpClientFactory f, IWebHostEnvironment env) : base(f, env) { }

        public async Task<IActionResult> Index()
            => View(await GetAsync<List<AdminArtistDto>>("api/admin/artists") ?? new());

        [HttpGet]
        public IActionResult Create() { LoadImageOptions(); return View("Form", new AdminArtistDto()); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminArtistDto dto)
        {
            if (!ModelState.IsValid) { LoadImageOptions(); return View("Form", dto); }
            var (ok, result) = await SendAsync(HttpMethod.Post, "api/admin/artists", dto);
            if (ok) Success(result?.Message); else Error(result?.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var artist = await GetAsync<AdminArtistDto>($"api/admin/artists/{id}");
            if (artist == null) return NotFound();
            LoadImageOptions();
            return View("Form", artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminArtistDto dto)
        {
            if (!ModelState.IsValid) { LoadImageOptions(); return View("Form", dto); }
            var (ok, result) = await SendAsync(HttpMethod.Put, $"api/admin/artists/{id}", dto);
            if (ok) Success(result?.Message); else Error(result?.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, result) = await SendAsync(HttpMethod.Delete, $"api/admin/artists/{id}");
            if (ok) Success(result?.Message); else Error(result?.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
