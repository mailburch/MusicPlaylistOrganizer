namespace MusicPlaylistOrganizer.Models.Api
{
    public class ArtistApiResult
    {
        public string ArtistName { get; set; } = string.Empty;
        public int ArtistId { get; set; }
        public string? ArtworkUrl100 { get; set; }

        public string? PrimaryGenreName { get; set; }
    }
}
