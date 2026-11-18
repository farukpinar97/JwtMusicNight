using JwtMusicNight.WebUI.Areas.Admin.Models;
using JwtMusicNight.WebUI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JwtMusicNight.WebUI.Areas.Admin.Controllers
{
    // Admin sayfalarının tabanı (asıl yetki kontrolü API'de)
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public abstract class AdminBaseController : MusicBaseController
    {
        private readonly IWebHostEnvironment _env;

        protected AdminBaseController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
            : base(httpClientFactory)
        {
            _env = env;
        }

        // POST / PUT / DELETE: (başarılı mı, API mesajı)
        protected async Task<(bool ok, ApiMessageDto? result)> SendAsync(HttpMethod method, string url, object? body = null)
        {
            var request = new HttpRequestMessage(method, url);
            if (body != null) request.Content = JsonContent.Create(body);

            var response = await Api.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApiUnauthorizedException();

            ApiMessageDto? result = null;
            try { result = await response.Content.ReadFromJsonAsync<ApiMessageDto>(); } catch { }

            return (response.IsSuccessStatusCode, result);
        }

        protected void Success(string? message) => TempData["Success"] = message;
        protected void Error(string? message) => TempData["Error"] = message ?? "İşlem başarısız.";

        // POST /Admin/{Songs|Artists|Genres}/UploadImage
        // Görseli wwwroot/assets/img/uploads içine kaydeder, yolunu döner
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(MaxImageSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxImageSize)]
        public async Task<IActionResult> UploadImage(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Dosya seçilmedi." });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedImageTypes.Contains(ext))
                return BadRequest(new { message = "Sadece JPG, PNG veya WEBP yüklenebilir." });

            if (file.Length > MaxImageSize)
                return BadRequest(new { message = "Görsel en fazla 5 MB olabilir." });

            var folder = Path.Combine(_env.WebRootPath, "assets", "img", "uploads");
            Directory.CreateDirectory(folder);

            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..23] + ext;
            await using (var stream = System.IO.File.Create(Path.Combine(folder, fileName)))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { url = "/assets/img/uploads/" + fileName });
        }

        private const long MaxImageSize = 5 * 1024 * 1024;
        private static readonly HashSet<string> AllowedImageTypes = new() { ".jpg", ".jpeg", ".png", ".webp" };

        // wwwroot/assets/img içindeki görseller (formlarda seçim listesi için)
        protected void LoadImageOptions()
        {
            var folder = Path.Combine(_env.WebRootPath, "assets", "img");
            var images = new List<string>();
            if (Directory.Exists(folder))
            {
                // Boş tema yer tutucularını atla
                images.AddRange(Directory.GetFiles(folder, "*.jpg")
                    .Where(f => new FileInfo(f).Length > 1024)
                    .Select(f => "/assets/img/" + Path.GetFileName(f))
                    .OrderBy(f => f.Length).ThenBy(f => f));

                var uploads = Path.Combine(folder, "uploads");
                if (Directory.Exists(uploads))
                    images.InsertRange(0, Directory.GetFiles(uploads)
                        .OrderByDescending(System.IO.File.GetCreationTime)
                        .Select(f => "/assets/img/uploads/" + Path.GetFileName(f)));
            }
            ViewBag.Images = images;
        }
    }
}
