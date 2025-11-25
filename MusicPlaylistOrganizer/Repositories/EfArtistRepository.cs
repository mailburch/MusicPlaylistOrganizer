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

        // GET ALL ARTISTS
        public async Task<List<Artist>> GetAllAsync() =>
            await _context.Artists
                .Include(a => a.Tracks)
                .OrderBy(a => a.Name)
                .ToListAsync();

        // GET BY ID (with tracks)
        public async Task<Artist?> GetByIdAsync(int id) =>
            await _context.Artists
                .Include(a => a.Tracks)
                .FirstOrDefaultAsync(a => a.ArtistID == id);

        // GET BY API SOURCE ID
        public async Task<Artist?> GetByApiSourceIdAsync(string apiSourceId)
        {
            return await _context.Artists
                .Include(a => a.Tracks)
                .FirstOrDefaultAsync(a => a.ApiSourceId == apiSourceId);
        }

        // 🔹 SMART CREATE OR RETURN EXISTING FROM API
        // 1) Try by ApiSourceId
        // 2) If not found, try by Name (case-insensitive)
        // 3) If found by name, attach ApiSourceId if missing
        // 4) Else create new
        public async Task<Artist> GetOrCreateFromApiAsync(string apiSourceId, string name, string? genre = null)
        {
            var trimmedName = name?.Trim() ?? string.Empty;

            // 1) Try by API source ID if we have one
            if (!string.IsNullOrWhiteSpace(apiSourceId))
            {
                var byApi = await GetByApiSourceIdAsync(apiSourceId);
                if (byApi != null)
                    return byApi;
            }

            // 2) Try by name
            Artist? byName = null;
            if (!string.IsNullOrWhiteSpace(trimmedName))
            {
                byName = await GetByNameAsync(trimmedName);
            }

            if (byName != null)
            {
                // 3) Attach ApiSourceId if we now know it and it's not set yet
                if (!string.IsNullOrWhiteSpace(apiSourceId) && string.IsNullOrEmpty(byName.ApiSourceId))
                {
                    byName.ApiSourceId = apiSourceId;
                    // Optionally update genre if we didn't have one
                    if (string.IsNullOrEmpty(byName.Genre) && !string.IsNullOrEmpty(genre))
                    {
                        byName.Genre = genre;
                    }

                    await _context.SaveChangesAsync();
                }

                return byName;
            }

            // 4) Create new artist
            var artist = new Artist
            {
                Name = trimmedName,
                Genre = genre,
                ApiSourceId = apiSourceId
                // ArtworkUrl intentionally NOT set here (track import handles it)
            };

            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            return artist;
        }

        // CREATE (manual form create)
        public async Task AddAsync(Artist artist)
        {
            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
        }

        // UPDATE
        public async Task UpdateAsync(Artist artist)
        {
            _context.Artists.Update(artist);
            await _context.SaveChangesAsync();
        }

        // DELETE
        public async Task DeleteAsync(int id)
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.ArtistID == id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
                await _context.SaveChangesAsync();
            }
        }

        // 🔹 LOOKUP BY NAME (case-insensitive)
        public async Task<Artist?> GetByNameAsync(string name)
        {
            var normalized = name.Trim().ToLower();
            return await _context.Artists
                .FirstOrDefaultAsync(a => a.Name.ToLower() == normalized);
        }

        // 🔹 GET OR CREATE BY NAME (used for multi-artist split)
        public async Task<Artist> GetOrCreateByNameAsync(string name, string? artworkUrl = null, string? genre = null)
        {
            var trimmedName = name.Trim();
            var existing = await GetByNameAsync(trimmedName);
            if (existing != null)
            {
                // If we didn’t have artwork yet and now we do, update it
                if (string.IsNullOrEmpty(existing.ArtworkUrl) && !string.IsNullOrEmpty(artworkUrl))
                {
                    existing.ArtworkUrl = artworkUrl;
                    await _context.SaveChangesAsync();
                }

                // Also optionally update genre if missing
                if (string.IsNullOrEmpty(existing.Genre) && !string.IsNullOrEmpty(genre))
                {
                    existing.Genre = genre;
                    await _context.SaveChangesAsync();
                }

                return existing;
            }

            var artist = new Artist
            {
                Name = trimmedName,
                Genre = genre,
                ArtworkUrl = artworkUrl,
                ApiSourceId = null // no specific API id for split/name-only artists
            };

            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();
            return artist;
        }
    }
}
