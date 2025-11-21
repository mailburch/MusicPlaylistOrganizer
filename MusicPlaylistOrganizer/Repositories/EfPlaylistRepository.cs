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

        public async Task<Playlist?> GetByIdAsync(int id)
        {
            return await _context.Playlists
                .FirstOrDefaultAsync(p => p.PlaylistID == id);
        }

        public async Task<Playlist?> GetWithTracksAsync(int id)
        {
            return await _context.Playlists
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track)
                        .ThenInclude(t => t.Artist)
                .FirstOrDefaultAsync(p => p.PlaylistID == id);
        }

        public Task<int> GetArtistCountAsync() => _context.Artists.CountAsync();
        public Task<int> GetTrackCountAsync() => _context.Tracks.CountAsync();
        public Task<int> GetPlaylistCountAsync() => _context.Playlists.CountAsync();

        public async Task AddAsync(Playlist playlist)
        {
            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Playlist playlist)
        {
            _context.Playlists.Update(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddTrackToPlaylistAsync(int playlistId, int trackId)
        {
            // 1️⃣ Check if this playlist already contains this track
            bool alreadyExists = await _context.PlaylistTracks
                .AnyAsync(pt => pt.PlaylistID == playlistId && pt.TrackID == trackId);

            if (alreadyExists)
            {
                // Optional: quietly return, or throw, or return some status
                return;
            }

            // 2️⃣ Determine next position
            int nextPosition = await _context.PlaylistTracks
                .Where(pt => pt.PlaylistID == playlistId)
                .CountAsync() + 1;

            // 3️⃣ Add the new track
            var ptNew = new PlaylistTrack
            {
                PlaylistID = playlistId,
                TrackID = trackId,
                Position = nextPosition
            };

            _context.PlaylistTracks.Add(ptNew);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateTrackOrderAsync(int playlistId, IList<int> trackIdsInOrder)
        {
            var pts = await _context.PlaylistTracks
                .Where(pt => pt.PlaylistID == playlistId)
                .ToListAsync();

            var lookup = pts.ToDictionary(pt => pt.TrackID);

            int position = 1;
            foreach (var trackId in trackIdsInOrder)
            {
                if (lookup.TryGetValue(trackId, out var pt))
                {
                    pt.Position = position++;
                }
            }

            await _context.SaveChangesAsync();
        }
        public async Task RemoveTrackFromPlaylistAsync(int playlistId, int trackId)
        {
            var pt = await _context.PlaylistTracks
                .FirstOrDefaultAsync(p => p.PlaylistID == playlistId && p.TrackID == trackId);

            if (pt != null)
            {
                _context.PlaylistTracks.Remove(pt);
                await _context.SaveChangesAsync();
            }
        }
    }
}
