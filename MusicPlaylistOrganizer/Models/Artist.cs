namespace MusicPlaylistOrganizer.Models
{
    public class Artist
    {
        public int ArtistID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Genre { get; set; }

        public string? ApiSourceId { get; set; }
        public string? ArtworkUrl { get; set; }

        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
