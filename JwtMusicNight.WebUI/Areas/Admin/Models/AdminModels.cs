using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JwtMusicNight.WebUI.Areas.Admin.Models
{
    public static class Packages
    {
        public static readonly string[] All = { "Basic", "Gold", "Premium", "Elit" };
    }

    // ---------- DASHBOARD ----------
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalSongs { get; set; }
        public int TotalArtists { get; set; }
        public int TotalGenres { get; set; }
        public int TotalListens { get; set; }
        public Dictionary<string, int> UsersByPackage { get; set; } = new();
        public Dictionary<string, int> SongsByPackage { get; set; } = new();
        public List<DailyListenDto> ListensLast7Days { get; set; } = new();
        public List<TopItemDto> TopSongs { get; set; } = new();
        public List<TopItemDto> TopArtists { get; set; } = new();
    }

    public class DailyListenDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class TopItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? SubTitle { get; set; }
        public string? ImageUrl { get; set; }
        public int ListenCount { get; set; }
    }

    // ---------- KULLANICI ----------
    public class AdminUserDto
    {
        public string Id { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public string? Email { get; set; }
        public string PackageLevel { get; set; } = "Basic";
        public int ListenCount { get; set; }
        public bool IsAdmin { get; set; }
    }

    // ---------- ŞARKI ----------
    public class AdminSongDto
    {
        public int SongId { get; set; }

        [Required(ErrorMessage = "Şarkı adı zorunludur.")]
        [Display(Name = "Şarkı adı")]
        public string SongName { get; set; } = "";

        [Display(Name = "Kapak görseli")]
        public string? CoverImageUrl { get; set; }

        public string? AudioUrl { get; set; }

        public TimeSpan Duration { get; set; }

        // Formda "3:25" şeklinde girilir, API'ye TimeSpan gider
        [JsonIgnore]
        [Display(Name = "Süre (dk:sn)")]
        [RegularExpression(@"^\d{1,2}:\d{2}$", ErrorMessage = "Süre dk:sn formatında olmalı (ör. 3:25).")]
        public string DurationText
        {
            get => Duration == TimeSpan.Zero ? "" : $"{(int)Duration.TotalMinutes}:{Duration.Seconds:00}";
            set
            {
                var parts = (value ?? "").Split(':');
                if (parts.Length == 2 && int.TryParse(parts[0], out var m) && int.TryParse(parts[1], out var s))
                    Duration = new TimeSpan(0, m, s);
            }
        }

        [Display(Name = "Çıkış tarihi")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; } = DateTime.Today;

        [Display(Name = "Paket seviyesi")]
        public string RequiredPackage { get; set; } = "Basic";

        [Display(Name = "Şarkı sözleri")]
        public string? Lyrics { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sanatçı seçiniz.")]
        [Display(Name = "Sanatçı")]
        public int ArtistId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Tür seçiniz.")]
        [Display(Name = "Tür")]
        public int GenreId { get; set; }

        public string? ArtistName { get; set; }
        public string? GenreName { get; set; }
        public int ListenCount { get; set; }
        public bool HasAudioFile { get; set; }
    }

    // ---------- SANATÇI ----------
    public class AdminArtistDto
    {
        public int ArtistId { get; set; }

        [Required(ErrorMessage = "Sanatçı adı zorunludur.")]
        [Display(Name = "Sanatçı adı")]
        public string ArtistName { get; set; } = "";

        [Display(Name = "Profil görseli")]
        public string? ArtistImageUrl { get; set; }

        [Display(Name = "Kapak (banner) görseli")]
        public string? CoverImageUrl { get; set; }

        [Display(Name = "Biyografi")]
        public string? Bio { get; set; }

        [Display(Name = "Ülke")]
        public string? Country { get; set; }

        [Display(Name = "Doğrulanmış sanatçı")]
        public bool IsVerified { get; set; }

        public int SongCount { get; set; }
    }

    // ---------- TÜR ----------
    public class AdminGenreDto
    {
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Tür adı zorunludur.")]
        [Display(Name = "Tür adı")]
        public string GenreName { get; set; } = "";

        [Display(Name = "Görsel")]
        public string? ImageUrl { get; set; }

        public int SongCount { get; set; }
    }

    // API'nin { message: "..." } cevapları
    public class ApiMessageDto
    {
        public string? Message { get; set; }
        public int? SongId { get; set; }
    }
}
