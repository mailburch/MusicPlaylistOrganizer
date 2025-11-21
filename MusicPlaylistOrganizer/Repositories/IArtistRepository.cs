using MusicPlaylistOrganizer.Models;

namespace MusicPlaylistOrganizer.Repositories
{
    public interface IArtistRepository
    {
        Task<List<Artist>> GetAllAsync();
        Task<Artist?> GetByIdAsync(int id);

        Task<Artist?> GetByApiSourceIdAsync(string apiSourceId);
        Task<Artist> GetOrCreateFromApiAsync(string apiSourceId, string name, string? country = null);

        Task AddAsync(Artist artist);
        Task UpdateAsync(Artist artist);
        Task DeleteAsync(int id);
        Task<Artist?> GetByNameAsync(string name);
        Task<Artist> GetOrCreateByNameAsync(string name, string? artworkUrl = null, string? country = null);

    }
}
