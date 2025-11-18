using MusicPlaylistOrganizer.Models;

namespace MusicPlaylistOrganizer.Repositories
{
    public interface IPlaylistRepository
    {
        Task<List<Playlist>> GetAllAsync();
        Task<int> GetArtistCountAsync();
        Task<int> GetTrackCountAsync();
        Task<int> GetPlaylistCountAsync();
    }
}
