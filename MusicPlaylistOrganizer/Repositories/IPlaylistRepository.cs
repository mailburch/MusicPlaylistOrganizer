using MusicPlaylistOrganizer.Models;

namespace MusicPlaylistOrganizer.Repositories
{
    public interface IPlaylistRepository
    {
        Task<Playlist?> GetByIdAsync(int id);
        Task AddAsync(Playlist playlist);
        Task UpdateAsync(Playlist playlist);
        Task DeleteAsync(int id);
        Task<List<Playlist>> GetAllAsync();
        Task<int> GetArtistCountAsync();
        Task<int> GetTrackCountAsync();
        Task<int> GetPlaylistCountAsync();
        Task UpdateTrackOrderAsync(int playlistId, IList<int> trackIdsInOrder);
        Task AddTrackToPlaylistAsync(int playlistId, int trackId);
        Task RemoveTrackFromPlaylistAsync(int playlistId, int trackId);
        Task<Playlist?> GetWithTracksAsync(int id);
    }
}
