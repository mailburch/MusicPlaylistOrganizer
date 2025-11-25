namespace MusicPlaylistOrganizer.Models.Api
{
    public class ImportTrackFromApiViewModel
    {
        public string TrackApiId { get; set; } = string.Empty;
        public string ArtistApiId { get; set; } = string.Empty;
        public string TrackName { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public int DurationMillis { get; set; }
        public string? ArtworkUrl { get; set; } = string.Empty;
        public string? GenreName { get; set; }

        //  If provided, we also add to this playlist
        public int? PlaylistId { get; set; }
    }
}
