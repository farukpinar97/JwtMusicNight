using JwtMusicNight.WebApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace JwtMusicNight.WebApi.Dtos
{
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

    // ---------- KULLANICILAR ----------
    public class AdminUserDto
    {
        public string Id { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public string? Email { get; set; }
        public PackageLevel PackageLevel { get; set; }
        public int ListenCount { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class UpdatePackageDto
    {
        [Required]
        public PackageLevel PackageLevel { get; set; }
    }

    // ---------- ŞARKI ----------
    public class AdminSongDto
    {
        public int SongId { get; set; }

        [Required(ErrorMessage = "Şarkı adı zorunludur.")]
        public string SongName { get; set; } = "";

        public string? CoverImageUrl { get; set; }
        public string? AudioUrl { get; set; }          // MP3 ayrı endpoint ile yüklenir
        public TimeSpan Duration { get; set; }
        public DateTime ReleaseDate { get; set; } = DateTime.Today;
        public PackageLevel RequiredPackage { get; set; } = PackageLevel.Basic;
        public string? Lyrics { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sanatçı seçiniz.")]
        public int ArtistId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Tür seçiniz.")]
        public int GenreId { get; set; }

        // sadece listelemede dolu
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
        public string ArtistName { get; set; } = "";

        public string? ArtistImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Bio { get; set; }
        public string? Country { get; set; }
        public bool IsVerified { get; set; }
        public int SongCount { get; set; }
    }

    // ---------- TÜR ----------
    public class AdminGenreDto
    {
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Tür adı zorunludur.")]
        public string GenreName { get; set; } = "";

        public string? ImageUrl { get; set; }
        public int SongCount { get; set; }
    }
}
