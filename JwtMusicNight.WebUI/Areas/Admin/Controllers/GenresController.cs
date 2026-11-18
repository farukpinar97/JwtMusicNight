using JwtMusicNight.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebUI.Areas.Admin.Controllers
{
    public class GenresController : AdminBaseController
    {
        public GenresController(IHttpClientFactory f, IWebHostEnvironment env) : base(f, env) { }

        public async Task<IActionResult> Index()
            => View(await GetAsync<List<AdminGenreDto>>("api/admin/genres") ?? new());

        [HttpGet]
        public IActionResult Create() { LoadImageOptions(); return View("Form", new AdminGenreDto()); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminGenreDto dto)
        {
            if (!ModelState.IsValid) { LoadImageOptions(); return View("Form", dto); }
            var (ok, result) = await SendAsync(HttpMethod.Post, "api/admin/genres", dto);
            if (ok) Success(result?.Message); else Error(result?.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var genre = await GetAsync<AdminGenreDto>($"api/admin/genres/{id}");
            if (genre == null) return NotFound();
            LoadImageOptions();
            return View("Form", genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminGenreDto dto)
        {
            if (!ModelState.IsValid) { LoadImageOptions(); return View("Form", dto); }
            var (ok, result) = await SendAsync(HttpMethod.Put, $"api/admin/genres/{id}", dto);
            if (ok) Success(result?.Message); else Error(result?.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, result) = await SendAsync(HttpMethod.Delete, $"api/admin/genres/{id}");
            if (ok) Success(result?.Message); else Error(result?.Message);
            return RedirectToAction(nameof(Index));
        }
    }
}
