namespace JwtMusicNight.WebApi.Dtos
{
    // Sanatçı detay sayfası: sanatçı bilgisi + tüm şarkıları
    public class ResultArtistDetailDto : ResultArtistDto
    {
        public int TotalListenCount { get; set; }
        public List<ResultSongDto> Songs { get; set; } = new();
    }
}
