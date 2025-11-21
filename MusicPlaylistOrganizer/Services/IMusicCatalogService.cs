using MusicPlaylistOrganizer.Models.Api;

namespace MusicPlaylistOrganizer.Services
{
    public interface IMusicCatalogService
    {
        Task<List<ArtistApiResult>> SearchArtistsAsync(string query);
        Task<List<TrackApiResult>> SearchTracksAsync(string query);
    }
}
