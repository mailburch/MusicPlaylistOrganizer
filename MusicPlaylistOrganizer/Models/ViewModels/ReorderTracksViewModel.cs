namespace MusicPlaylistOrganizer.Models
{
    public class ReorderTracksViewModel
    {
        public int PlaylistId { get; set; }
        public List<int> TrackIds { get; set; } = new();
    }
}
