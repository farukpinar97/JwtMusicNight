using JwtMusicNight.WebApi.Context;
using JwtMusicNight.WebApi.Dtos;
using JwtMusicNight.WebApi.Services.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtMusicNight.WebApi.Controllers.Admin
{
    [Route("api/admin/songs")]
    [ApiController]
    [Authorize(Roles = AdminSeeder.AdminRole)]
    public class AdminSongsController : ControllerBase
    {
        private const long MaxAudioSize = 50 * 1024 * 1024; // 50 MB

        private readonly IAdminCatalogService _service;
        private readonly IWebHostEnvironment _env;

        public AdminSongsController(IAdminCatalogService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search) => Ok(await _service.GetSongsAsync(search));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var song = await _service.GetSongAsync(id);
            return song == null ? NotFound(new { message = "Şarkı bulunamadı." }) : Ok(song);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminSongDto dto)
        {
            var id = await _service.CreateSongAsync(dto);
            return Ok(new { songId = id, message = "Şarkı eklendi. Şimdi MP3 dosyasını yükleyebilirsiniz." });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, AdminSongDto dto)
        {
            var ok = await _service.UpdateSongAsync(id, dto);
            return ok ? Ok(new { message = "Şarkı güncellendi." }) : NotFound(new { message = "Şarkı bulunamadı." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fileName = await _service.DeleteSongAsync(id);
            if (fileName == null) return NotFound(new { message = "Şarkı bulunamadı." });

            // MP3 dosyasını da sil
            if (!string.IsNullOrEmpty(fileName))
            {
                var path = Path.Combine(_env.ContentRootPath, "Musics", fileName);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            return Ok(new { message = "Şarkı silindi." });
        }

        // POST api/admin/songs/5/audio   (form-data: file = xxx.mp3)
        [HttpPost("{id:int}/audio")]
        [RequestSizeLimit(MaxAudioSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxAudioSize)]
        public async Task<IActionResult> UploadAudio(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Dosya seçilmedi." });

            if (!Path.GetExtension(file.FileName).Equals(".mp3", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Sadece .mp3 dosyası yüklenebilir." });

            if (await _service.GetSongAsync(id) == null)
                return NotFound(new { message = "Şarkı bulunamadı." });

            var folder = Path.Combine(_env.ContentRootPath, "Musics");
            Directory.CreateDirectory(folder);

            var fileName = $"song{id:00}.mp3";          // seeder ile aynı isimlendirme
            await using (var stream = System.IO.File.Create(Path.Combine(folder, fileName)))
            {
                await file.CopyToAsync(stream);
            }

            await _service.SetAudioFileAsync(id, fileName);
            return Ok(new { message = "MP3 yüklendi.", fileName });
        }
    }
}
