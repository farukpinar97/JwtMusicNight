namespace JwtMusicNight.WebUI.Dtos
{
    public class ResultGenreDto
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public string? ImageUrl { get; set; }
        public int SongCount { get; set; }
    }
}
