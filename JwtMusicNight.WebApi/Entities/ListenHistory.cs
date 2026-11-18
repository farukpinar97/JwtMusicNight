namespace JwtMusicNight.WebApi.Entities
{
    public class ListenHistory
    {
        public int ListenHistoryId { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int SongId { get; set; }
        public Song Song { get; set; }
        public DateTime ListenedAt { get; set; } = DateTime.Now;
    }
}