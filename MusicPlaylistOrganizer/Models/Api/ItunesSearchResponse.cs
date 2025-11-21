namespace MusicPlaylistOrganizer.Models.Api
{
    // Generic wrapper for iTunes "search" JSON response
    public class ItunesSearchResponse<T>
    {
        public int ResultCount { get; set; }
        public List<T> Results { get; set; } = new();
    }
}
