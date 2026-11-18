namespace JwtMusicNight.WebApi.Dtos
{
    // "Bunları da sevebilirsiniz" cevabı
    public class ForYouResultDto
    {
        // "ML.NET" -> model tahmini, "Popular" -> yeterli veri yoksa en popülerler
        public string Source { get; set; } = "ML.NET";
        public List<ResultSongDto> Songs { get; set; } = new();
    }
}
