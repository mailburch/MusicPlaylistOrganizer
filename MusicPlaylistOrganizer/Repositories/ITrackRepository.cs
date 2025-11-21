using MusicPlaylistOrganizer.Models;

namespace MusicPlaylistOrganizer.Repositories
{
    public interface ITrackRepository
    {
        Task<List<Track>> GetAllAsync();
        Task<Track?> GetByIdAsync(int id);

        Task<Track?> GetByApiSourceIdAsync(string apiSourceId);
        Task<Track> GetOrCreateFromApiAsync(
            string apiSourceId,
            string title,
            int durationSeconds,
            int artistId,
            string? artworkUrl = null
        );

        Task AddAsync(Track track);
        Task UpdateAsync(Track track);
        Task DeleteAsync(int id);
    }
}
