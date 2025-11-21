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

        // All tracks (for index)
        public async Task<List<Track>> GetAllAsync() =>
            await _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.PlaylistTracks)
                    .ThenInclude(pt => pt.Playlist)
                .OrderBy(t => t.Title)
                .ToListAsync();

        // Single track with nav props (for Details/Edit/Delete)
        public async Task<Track?> GetByIdAsync(int id) =>
            await _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.PlaylistTracks)
                    .ThenInclude(pt => pt.Playlist)
                .FirstOrDefaultAsync(t => t.TrackID == id);

        public async Task<Track?> GetByApiSourceIdAsync(string apiSourceId)
        {
            return await _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.PlaylistTracks)
                    .ThenInclude(pt => pt.Playlist)
                .FirstOrDefaultAsync(t => t.ApiSourceId == apiSourceId);
        }

        public async Task<Track> GetOrCreateFromApiAsync(
            string apiSourceId,
            string title,
            int durationSeconds,
            int artistId,
            string? artworkUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(apiSourceId))
            {
                var existing = await GetByApiSourceIdAsync(apiSourceId);
                if (existing != null)
                    return existing;
            }

            var track = new Track
            {
                Title = title,
                DurationSeconds = durationSeconds,
                ArtistID = artistId,
                ApiSourceId = apiSourceId,
                ArtworkUrl = artworkUrl
            };

            _context.Tracks.Add(track);
            await _context.SaveChangesAsync();
            return track;
        }

        public async Task AddAsync(Track track)
        {
            _context.Tracks.Add(track);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Track track)
        {
            _context.Tracks.Update(track);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var track = await _context.Tracks.FirstOrDefaultAsync(t => t.TrackID == id);
            if (track != null)
            {
                _context.Tracks.Remove(track);
                await _context.SaveChangesAsync();
            }
        }
    }
}
