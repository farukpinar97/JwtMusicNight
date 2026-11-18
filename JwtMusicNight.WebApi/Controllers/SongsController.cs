using JwtMusicNight.WebApi.Entities;
using JwtMusicNight.WebApi.Services.SongServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtMusicNight.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Token yoksa / geçersizse -> 401 Unauthorized
    public class SongsController : ControllerBase
    {
        private const string UpgradeMessage =
            "Mevcut paketiniz bu şarkıyı desteklememektedir. Lütfen paketinizi yükseltin.";

        private readonly ISongService _songService;
        private readonly IWebHostEnvironment _env;

        public SongsController(ISongService songService, IWebHostEnvironment env)
        {
            _songService = songService;
            _env = env;
        }

        // GET api/songs -> tüm şarkılar
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _songService.GetAllSongsAsync());

        // GET api/songs/charts?genreId=5&sort=popular&count=20
        [HttpGet("charts")]
        public async Task<IActionResult> GetCharts([FromQuery] int? genreId, [FromQuery] string sort = "popular", [FromQuery] int count = 20)
            => Ok(await _songService.GetChartsAsync(genreId, sort, Math.Clamp(count, 1, 50)));

        // GET api/songs/5 -> şarkı detayı
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);
            return song == null ? NotFound(new { message = "Şarkı bulunamadı." }) : Ok(song);
        }

        // GET api/songs/artist/3 -> sanatçının şarkıları
        [HttpGet("artist/{artistId:int}")]
        public async Task<IActionResult> GetByArtist(int artistId)
            => Ok(await _songService.GetSongsByArtistAsync(artistId));

        // GET api/songs/genre/2 -> türe göre şarkılar
        [HttpGet("genre/{genreId:int}")]
        public async Task<IActionResult> GetByGenre(int genreId)
            => Ok(await _songService.GetSongsByGenreAsync(genreId));

        // GET api/songs/5/recommendations -> Önerilenler
        [HttpGet("{id:int}/recommendations")]
        public async Task<IActionResult> GetRecommendations(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _songService.GetRecommendationsAsync(id, userId));
        }

        // GET api/songs/history -> giriş yapan kullanıcının son dinledikleri
        [HttpGet("history")]
        public async Task<IActionResult> GetMyHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            return Ok(await _songService.GetUserHistoryAsync(userId));
        }

        // GET api/songs/5/stream -> paket kontrolü + MP3
        [HttpGet("{id:int}/stream")]
        public async Task<IActionResult> Stream(int id)
        {
            var song = await _songService.GetSongEntityAsync(id);
            if (song == null)
                return NotFound(new { message = "Şarkı bulunamadı." });

            // 1) Paket kontrolü: token'daki seviye < şarkının seviyesi ise 403
            var userPackage = GetUserPackage();
            if (userPackage < song.RequiredPackage)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = UpgradeMessage,
                    userPackage = userPackage.ToString(),
                    requiredPackage = song.RequiredPackage.ToString()
                });
            }

            // 2) Dosya kontrolü
            var filePath = Path.Combine(_env.ContentRootPath, "Musics", song.AudioUrl);
            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "MP3 dosyası sunucuda bulunamadı." });

            // 3) Dinlemeyi kaydet (sadece ilk parça isteğinde)
            if (IsFirstChunk())
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                    await _songService.AddListenAsync(song.SongId, userId);
            }

            // 4) MP3'ü gönder (Range destekli)
            return PhysicalFile(filePath, "audio/mpeg", enableRangeProcessing: true);
        }

        // Token'daki "packageLevel" claim'ini okur; yoksa / bozuksa en düşük paket
        private PackageLevel GetUserPackage()
        {
            var value = User.FindFirstValue("packageLevel");
            return int.TryParse(value, out var level) && Enum.IsDefined(typeof(PackageLevel), level)
                ? (PackageLevel)level
                : PackageLevel.Basic;
        }

        private bool IsFirstChunk()
        {
            var range = Request.Headers.Range.ToString();
            return string.IsNullOrEmpty(range) || range.StartsWith("bytes=0-");
        }
    }
}
