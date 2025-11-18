using Microsoft.EntityFrameworkCore;
using MusicPlaylistOrganizer.Data;
using MusicPlaylistOrganizer.Models;

namespace MusicPlaylistOrganizer.Repositories
{
    public class EfTrackRepository : ITrackRepository
    {
        private readonly MusicContext _context;

        public EfTrackRepository(MusicContext context)
        {
            _context = context;
        }

        public Task<List<Track>> GetAllAsync() =>
            _context.Tracks.Include(t => t.Artist).ToListAsync();
    }
}
