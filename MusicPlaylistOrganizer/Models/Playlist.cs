namespace MusicPlaylistOrganizer.Models
{
    public class Playlist
    {
        public int PlaylistID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<PlaylistTrack> PlaylistTracks { get; set; } = new List<PlaylistTrack>();
    }
}
