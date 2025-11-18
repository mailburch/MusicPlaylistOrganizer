using Microsoft.EntityFrameworkCore;
using MusicPlaylistOrganizer.Data;
using MusicPlaylistOrganizer.Models;

namespace MusicPlaylistOrganizer.Repositories
{
    public class EfArtistRepository : IArtistRepository
    {
        private readonly MusicContext _context;

        public EfArtistRepository(MusicContext context)
        {
            _context = context;
        }

        public Task<List<Artist>> GetAllAsync() =>
            _context.Artists.OrderBy(a => a.Name).ToListAsync();

        public Task<Artist?> GetByIdAsync(int id) =>
            _context.Artists
                .Include(a => a.Tracks)
                .FirstOrDefaultAsync(a => a.ArtistID == id);
    }
}
