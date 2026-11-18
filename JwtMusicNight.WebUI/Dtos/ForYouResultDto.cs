namespace JwtMusicNight.WebUI.Dtos
{
    public class ForYouResultDto
    {
        public string Source { get; set; } = "ML.NET";   // "ML.NET" veya "Popular"
        public List<ResultSongDto> Songs { get; set; } = new();
    }
}
