using JwtMusicNight.WebApi.Entities;

namespace JwtMusicNight.WebApi.Dtos
{
    public class ResultSongDto
    {
        public int SongId { get; set; }
        public string SongName { get; set; }
        public string CoverImageUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public int ListenCount { get; set; }
        public DateTime ReleaseDate { get; set; }
        public PackageLevel RequiredPackage { get; set; }
        public string? Lyrics { get; set; }

        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public string? ArtistImageUrl { get; set; }       // UI: hero'daki küçük sanatçı avatarı
        public string? ArtistCoverImageUrl { get; set; }  // UI: hero arka planı (geniş banner)

        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }
}