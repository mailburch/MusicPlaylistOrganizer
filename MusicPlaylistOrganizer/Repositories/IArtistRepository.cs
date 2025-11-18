using MusicPlaylistOrganizer.Models;

namespace MusicPlaylistOrganizer.Repositories
{
    public interface IArtistRepository
    {
        Task<List<Artist>> GetAllAsync();
        Task<Artist?> GetByIdAsync(int id);
    }
}
