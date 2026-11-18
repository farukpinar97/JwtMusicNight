using JwtMusicNight.WebUI.Dtos;

namespace JwtMusicNight.WebUI.Models
{
    public class MusicIndexViewModel
    {
        public List<ResultSongDto> Featured { get; set; } = new();   // slider: en çok dinlenen 3 şarkı
        public List<ResultSongDto> Songs { get; set; } = new();
        public List<ResultSongDto> History { get; set; } = new();
        public ForYouResultDto ForYou { get; set; } = new();          // ML.NET: "Bunları da sevebilirsiniz"
    }

    public class SongDetailViewModel
    {
        public ResultSongDto Song { get; set; }
        public List<ResultSongDto> Recommendations { get; set; } = new();
        public List<ResultSongDto> ArtistSongs { get; set; } = new();
    }

    public class GenreDetailViewModel
    {
        public ResultGenreDto Genre { get; set; }
        public List<ResultSongDto> Songs { get; set; } = new();
        public List<ResultGenreDto> AllGenres { get; set; } = new();
    }

    public class ChartViewModel
    {
        public List<ResultSongDto> Songs { get; set; } = new();
        public List<ResultGenreDto> Genres { get; set; } = new();
        public int? SelectedGenreId { get; set; }
        public string Sort { get; set; } = "popular";
    }
}
