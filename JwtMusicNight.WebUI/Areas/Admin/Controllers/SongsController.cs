using JwtMusicNight.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;

namespace JwtMusicNight.WebUI.Areas.Admin.Controllers
{
    public class SongsController : AdminBaseController
    {
        private const long MaxAudioSize = 50 * 1024 * 1024;

        public SongsController(IHttpClientFactory f, IWebHostEnvironment env) : base(f, env) { }

        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            return View(await GetAsync<List<AdminSongDto>>($"api/admin/songs?search={Uri.EscapeDataString(search ?? "")}") ?? new());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadFormDataAsync();
            return View("Form", new AdminSongDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminSongDto dto)
        {
            if (!ModelState.IsValid) { await LoadFormDataAsync(); return View("Form", dto); }

            var (ok, result) = await SendAsync(HttpMethod.Post, "api/admin/songs", dto);
            if (!ok) { Error(result?.Message); await LoadFormDataAsync(); return View("Form", dto); }

            Success(result?.Message);
            // MP3 yüklemek için düzenleme sayfasına git
            return RedirectToAction(nameof(Edit), new { id = result?.SongId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var song = await GetAsync<AdminSongDto>($"api/admin/songs/{id}");
            if (song == null) return NotFound();
            await LoadFormDataAsync();
            return View("Form", song);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminSongDto dto)
        {
            if (!ModelState.IsValid) { await LoadFormDataAsync(); return View("Form", dto); }

            var (ok, result) = await SendAsync(HttpMethod.Put, $"api/admin/songs/{id}", dto);
            if (ok) Success(result?.Message); else Error(result?.Message);
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, result) = await SendAsync(HttpMethod.Delete, $"api/admin/songs/{id}");
            if (ok) Success(result?.Message); else Error(result?.Message);
            return RedirectToAction(nameof(Index));
        }

        // MP3'ü UI'dan alıp API'ye form-data olarak iletir
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(MaxAudioSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxAudioSize)]
        public async Task<IActionResult> UploadAudio(int id, IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                Error("Lütfen bir MP3 dosyası seçin.");
                return RedirectToAction(nameof(Edit), new { id });
            }

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
            content.Add(fileContent, "file", file.FileName);

            var response = await Api.PostAsync($"api/admin/songs/{id}/audio", content);
            ApiMessageDto? result = null;
            try { result = await response.Content.ReadFromJsonAsync<ApiMessageDto>(); } catch { }

            if (response.IsSuccessStatusCode) Success(result?.Message ?? "MP3 yüklendi.");
            else Error(result?.Message ?? "MP3 yüklenemedi.");

            return RedirectToAction(nameof(Edit), new { id });
        }

        private async Task LoadFormDataAsync()
        {
            var artists = await GetAsync<List<AdminArtistDto>>("api/admin/artists") ?? new();
            var genres = await GetAsync<List<AdminGenreDto>>("api/admin/genres") ?? new();
            ViewBag.Artists = new SelectList(artists, nameof(AdminArtistDto.ArtistId), nameof(AdminArtistDto.ArtistName));
            ViewBag.Genres = new SelectList(genres, nameof(AdminGenreDto.GenreId), nameof(AdminGenreDto.GenreName));
            ViewBag.Packages = new SelectList(Packages.All);
            LoadImageOptions();
        }
    }
}
