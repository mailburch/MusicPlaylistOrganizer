using Microsoft.EntityFrameworkCore;
using MusicPlaylistOrganizer.Data;
using MusicPlaylistOrganizer.Models;

namespace MusicPlaylistOrganizer.Repositories
{
    public class EfPlaylistRepository : IPlaylistRepository
    {
        private readonly MusicContext _context;

        public EfPlaylistRepository(MusicContext context)
        {
            _context = context;
        }

        public Task<List<Playlist>> GetAllAsync() =>
            _context.Playlists
                .Include(p => p.PlaylistTracks)
                .ThenInclude(pt => pt.Track)
                .ToListAsync();

        public Task<int> GetArtistCountAsync() => _context.Artists.CountAsync();
        public Task<int> GetTrackCountAsync() => _context.Tracks.CountAsync();
        public Task<int> GetPlaylistCountAsync() => _context.Playlists.CountAsync();
    }
}
