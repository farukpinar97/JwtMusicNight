using JwtMusicNight.WebUI.Dtos;

namespace JwtMusicNight.WebUI.Models
{
    // Hesabım & Paketim sayfası
    public class AccountViewModel
    {
        public List<ResultSongDto> Songs { get; set; } = new();
        public List<ResultSongDto> History { get; set; } = new();
    }

    // Arama sonuçları
    public class SearchViewModel
    {
        public string Query { get; set; } = "";
        public List<ResultSongDto> Songs { get; set; } = new();
        public List<ResultArtistDto> Artists { get; set; } = new();
    }
}
