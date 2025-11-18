using MusicPlaylistOrganizer.Models;

namespace MusicPlaylistOrganizer.Repositories
{
    public interface ITrackRepository
    {
        Task<List<Track>> GetAllAsync();
    }
}
