namespace JwtMusicNight.WebUI.Dtos
{
    public class ResultSongDto
    {
        public int SongId { get; set; }
        public string SongName { get; set; }
        public string CoverImageUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public int ListenCount { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string RequiredPackage { get; set; }   // "Basic", "Gold", "Premium", "Elit"
        public string? Lyrics { get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public string? ArtistImageUrl { get; set; }
        public string? ArtistCoverImageUrl { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }
}
