namespace MusicPlaylistOrganizer.Models
{
    public class PlaylistTrack
    {
        public int PlaylistID { get; set; }
        public Playlist Playlist { get; set; } = null!;

        public int TrackID { get; set; }
        public Track Track { get; set; } = null!;

        public int Position { get; set; }
    }
}
