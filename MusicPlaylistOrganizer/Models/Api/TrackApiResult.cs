namespace MusicPlaylistOrganizer.Models.Api
{
    public class TrackApiResult
    {
        public string TrackName { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;

        // Duration in milliseconds from iTunes
        public int TrackTimeMillis { get; set; }

        // Artwork URL (typically 100x100)
        public string ArtworkUrl100 { get; set; } = string.Empty;

        public int TrackId { get; set; }
        public int ArtistId { get; set; }
    }
}
