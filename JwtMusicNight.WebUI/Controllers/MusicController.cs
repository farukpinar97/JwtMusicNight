using JwtMusicNight.WebUI.Dtos;
using JwtMusicNight.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JwtMusicNight.WebUI.Controllers
{
    public class MusicController : MusicBaseController
    {
        public MusicController(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        // Login sonrası açılan sayfa: son dinlenenler + tüm şarkılar
        public async Task<IActionResult> Index()
        {
            var model = new MusicIndexViewModel
            {
                Featured = await GetAsync<List<ResultSongDto>>("api/songs/charts?sort=popular&count=3") ?? new(),
                Songs = await GetAsync<List<ResultSongDto>>("api/songs") ?? new(),
                History = await GetAsync<List<ResultSongDto>>("api/songs/history") ?? new(),
                ForYou = await GetAsync<ForYouResultDto>("api/songs/for-you") ?? new()
            };
            return View(model);
        }

        // Tekil şarkı detay sayfası + Önerilenler
        public async Task<IActionResult> Detail(int id)
        {
            var song = await GetAsync<ResultSongDto>($"api/songs/{id}");
            if (song == null) return NotFound();

            var model = new SongDetailViewModel
            {
                Song = song,
                Recommendations = await GetAsync<List<ResultSongDto>>($"api/songs/{id}/recommendations") ?? new(),
                ArtistSongs = (await GetAsync<List<ResultSongDto>>($"api/songs/artist/{song.ArtistId}") ?? new())
                                .Where(s => s.SongId != id).ToList()
            };
            return View(model);
        }

        // GET /Music/Search?q=pop  -> şarkı adı, sanatçı veya türe göre arama
        public async Task<IActionResult> Search(string? q)
        {
            var model = new SearchViewModel { Query = q?.Trim() ?? "" };

            if (model.Query.Length >= 2)
            {
                var songs = await GetAsync<List<ResultSongDto>>("api/songs") ?? new();
                var artists = await GetAsync<List<ResultArtistDto>>("api/artists") ?? new();

                // Türkçe karakter ve büyük/küçük harf duyarsız eşleştirme
                var compare = new System.Globalization.CultureInfo("tr-TR").CompareInfo;
                bool Match(string? text) => !string.IsNullOrEmpty(text) &&
                    compare.IndexOf(text, model.Query,
                        System.Globalization.CompareOptions.IgnoreCase | System.Globalization.CompareOptions.IgnoreNonSpace) >= 0;

                model.Songs = songs.Where(s => Match(s.SongName) || Match(s.ArtistName) || Match(s.GenreName)).ToList();
                model.Artists = artists.Where(a => Match(a.ArtistName) || Match(a.Country)).ToList();
            }

            return View(model);
        }

        // GET /Music/Stream/5 -> MP3'ü API'den alıp iletir
        [HttpGet]
        public async Task<IActionResult> Stream(int id)
        {
            var response = await Api.GetAsync($"api/songs/{id}/stream", HttpCompletionOption.ResponseHeadersRead);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return File(await response.Content.ReadAsStreamAsync(), "audio/mpeg");

                case HttpStatusCode.Forbidden:
                    return StatusCode(403, await response.Content.ReadFromJsonAsync<StreamErrorDto>());

                case HttpStatusCode.Unauthorized:
                    return StatusCode(401, new { message = "Oturum süreniz doldu. Lütfen tekrar giriş yapın." });

                case HttpStatusCode.NotFound:
                    return NotFound(new { message = "Şarkı dosyası bulunamadı." });

                default:
                    return StatusCode((int)response.StatusCode, new { message = "Şarkı çalınırken bir hata oluştu." });
            }
        }
    }
}
