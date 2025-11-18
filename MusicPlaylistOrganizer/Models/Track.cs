namespace MusicPlaylistOrganizer.Models
{
    public class Track
    {
        public int TrackID { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }

        public int ArtistID { get; set; }
        public Artist Artist { get; set; } = null!;

        public string? ApiSourceId { get; set; }
        public string? ArtworkUrl { get; set; }

        public ICollection<PlaylistTrack> PlaylistTracks { get; set; } = new List<PlaylistTrack>();
    }
}
