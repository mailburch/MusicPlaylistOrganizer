using Microsoft.AspNetCore.Mvc;
using MusicPlaylistOrganizer.Models;
using MusicPlaylistOrganizer.Models.Api;
using MusicPlaylistOrganizer.Repositories;
using MusicPlaylistOrganizer.Services;

namespace MusicPlaylistOrganizer.Controllers
{
    [Route("[controller]/[action]")]
    public class CatalogController : Controller
    {
        private readonly IArtistRepository _artistRepo;
        private readonly ITrackRepository _trackRepo;
        private readonly IPlaylistRepository _playlistRepo;
        private readonly IMusicCatalogService _catalog;

        public CatalogController(
            IArtistRepository artistRepo,
            ITrackRepository trackRepo,
            IPlaylistRepository playlistRepo,
            IMusicCatalogService catalog)
        {
            _artistRepo = artistRepo;
            _trackRepo = trackRepo;
            _playlistRepo = playlistRepo;
            _catalog = catalog;
        }

        // -------------------------------
        // SEARCH TRACKS (for Playlists)
        // -------------------------------
        [HttpGet]
        public async Task<IActionResult> SearchTracks(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(Array.Empty<object>());

            var results = await _catalog.SearchTracksAsync(query);

            // Shape the JSON to EXACTLY what your JS expects
            var shaped = results.Select(t => new
            {
                trackId = t.TrackId,
                trackName = t.TrackName,
                artistId = t.ArtistId,
                artistName = t.ArtistName,
                trackTimeMillis = t.TrackTimeMillis,
                artworkUrl100 = t.ArtworkUrl100
            });

            return Json(shaped);
        }

        // -------------------------------
        // SEARCH ARTISTS (for Artists pages)
        // -------------------------------
        [HttpGet]
        public async Task<IActionResult> SearchArtists(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(Array.Empty<object>());

            var results = await _catalog.SearchArtistsAsync(query);

            var shaped = results.Select(a => new
            {
                artistId = a.ArtistId,
                artistName = a.ArtistName,
                primaryGenreName = a.PrimaryGenreName,
                artworkUrl100 = a.ArtworkUrl100
            });

            return Json(shaped);
        }

        [HttpPost]
        public async Task<IActionResult> ImportTrack([FromBody] ImportTrackFromApiViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid import data.");

            var artistNameRaw = model.ArtistName ?? string.Empty;
            artistNameRaw = artistNameRaw.Trim();

            Artist primaryArtist;

            // 🔹 MULTIPLE ARTISTS: "Me & You"
            if (artistNameRaw.Contains("&"))
            {
                var parts = artistNameRaw
                    .Split('&', StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => n.Trim())
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .ToList();

                var artistsForThisTrack = new List<Artist>();

                foreach (var name in parts)
                {
                    // Reuse existing artists when possible, else create
                    var artist = await _artistRepo.GetOrCreateByNameAsync(
                        name,
                        artworkUrl: model.ArtworkUrl,
                        genre: null
                    );

                    artistsForThisTrack.Add(artist);
                }

                // Primary artist = first in the list ("Me" in "Me & You")
                primaryArtist = artistsForThisTrack.FirstOrDefault()
                                ?? await _artistRepo.GetOrCreateFromApiAsync(
                                       apiSourceId: model.ArtistApiId,
                                       name: artistNameRaw,
                                       genre: null
                                   );
            }
            else
            {
                // 🔹 SINGLE ARTIST: "You"
                // First try to reuse by name so "You" goes to existing row if present
                var existingByName = !string.IsNullOrWhiteSpace(artistNameRaw)
                    ? await _artistRepo.GetByNameAsync(artistNameRaw)
                    : null;

                if (existingByName != null)
                {
                    // If this existing artist doesn't have an ApiSourceId and we know one, attach it
                    if (!string.IsNullOrWhiteSpace(model.ArtistApiId) &&
                        string.IsNullOrEmpty(existingByName.ApiSourceId))
                    {
                        existingByName.ApiSourceId = model.ArtistApiId;
                        await _artistRepo.UpdateAsync(existingByName);
                    }

                    primaryArtist = existingByName;
                }
                else
                {
                    // Fall back to API-based creation
                    primaryArtist = await _artistRepo.GetOrCreateFromApiAsync(
                        apiSourceId: model.ArtistApiId,
                        name: artistNameRaw,
                        genre: null
                    );
                }
            }

            // 🔹 CREATE / GET TRACK
            int durationSeconds = model.DurationMillis / 1000;

            var track = await _trackRepo.GetOrCreateFromApiAsync(
                apiSourceId: model.TrackApiId,
                title: model.TrackName,
                durationSeconds: durationSeconds,
                artistId: primaryArtist.ArtistID,
                artworkUrl: model.ArtworkUrl
            );

            // 🔹 Attach to playlist if provided
            if (model.PlaylistId.HasValue)
            {
                await _playlistRepo.AddTrackToPlaylistAsync(
                    model.PlaylistId.Value,
                    track.TrackID
                );
            }

            return Json(new
            {
                success = true,
                artistId = primaryArtist.ArtistID,
                trackId = track.TrackID
            });
        }
    }
}
