using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicPlaylistOrganizer.Models;
using MusicPlaylistOrganizer.Repositories;

namespace MusicPlaylistOrganizer.Controllers
{
    public class TracksController : Controller
    {
        private readonly ITrackRepository _trackRepo;
        private readonly IArtistRepository _artistRepo;
        private readonly IPlaylistRepository _playlistRepo;

        public TracksController(
            ITrackRepository trackRepo,
            IArtistRepository artistRepo,
            IPlaylistRepository playlistRepo)
        {
            _trackRepo = trackRepo;
            _artistRepo = artistRepo;
            _playlistRepo = playlistRepo;
        }

        // GET: /Tracks
        public async Task<IActionResult> Index(string? sortOrder, string? searchString, int? artistFilter)
        {
            ViewData["TitleSortParm"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["ArtistSortParm"] = sortOrder == "artist" ? "artist_desc" : "artist";
            ViewData["DurationSortParm"] = sortOrder == "duration" ? "duration_desc" : "duration";

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentArtistFilter"] = artistFilter;

            var tracks = await _trackRepo.GetAllAsync(); // includes Artist + PlaylistTracks

            // search by title or artist
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.ToLower();
                tracks = tracks
                    .Where(t =>
                        t.Title.ToLower().Contains(term) ||
                        (t.Artist != null && t.Artist.Name.ToLower().Contains(term)))
                    .ToList();
            }

            // filter by artist
            if (artistFilter.HasValue && artistFilter.Value > 0)
            {
                tracks = tracks
                    .Where(t => t.ArtistID == artistFilter.Value)
                    .ToList();
            }

            // sort
            tracks = sortOrder switch
            {
                "title_desc" => tracks.OrderByDescending(t => t.Title).ToList(),

                "artist" => tracks
                    .OrderBy(t => t.Artist?.Name ?? "")
                    .ThenBy(t => t.Title)
                    .ToList(),

                "artist_desc" => tracks
                    .OrderByDescending(t => t.Artist?.Name ?? "")
                    .ThenBy(t => t.Title)
                    .ToList(),

                "duration" => tracks
                    .OrderBy(t => t.DurationSeconds)
                    .ThenBy(t => t.Title)
                    .ToList(),

                "duration_desc" => tracks
                    .OrderByDescending(t => t.DurationSeconds)
                    .ThenBy(t => t.Title)
                    .ToList(),

                _ => tracks.OrderBy(t => t.Title).ToList()
            };

            // artist list for filter dropdown
            var artists = await _artistRepo.GetAllAsync();
            ViewBag.ArtistOptions = artists
                .OrderBy(a => a.Name)
                .ToList();

            return View(tracks);
        }

        // helper for the Artist dropdown in Create/Edit
        private async Task PopulateArtistsDropDownList(int? selectedArtistId = null)
        {
            var artists = await _artistRepo.GetAllAsync();
            ViewBag.ArtistID = new SelectList(artists, "ArtistID", "Name", selectedArtistId);
        }

        // GET: /Tracks/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateArtistsDropDownList();
            return View();
        }

        // POST: /Tracks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,DurationSeconds,ArtworkUrl,ApiSourceId,ArtistID")] Track track)
        {
            if (!ModelState.IsValid)
            {
                await PopulateArtistsDropDownList(track.ArtistID);
                return View(track);
            }

            await _trackRepo.AddAsync(track);

            // After creating, jump straight to Details so user can tweak and add to playlists
            return RedirectToAction("Details", new { id = track.TrackID });
        }

        // GET: /Tracks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var track = await _trackRepo.GetByIdAsync(id);
            if (track == null) return NotFound();

            ViewBag.Playlists = await _playlistRepo.GetAllAsync();
            return View(track);
        }

        // GET: /Tracks/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var track = await _trackRepo.GetByIdAsync(id);
            if (track == null) return NotFound();

            await PopulateArtistsDropDownList(track.ArtistID);
            return View(track);
        }

        // POST: /Tracks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("TrackID,Title,DurationSeconds,ArtworkUrl,ApiSourceId,ArtistID")] Track track)
        {
            if (id != track.TrackID) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateArtistsDropDownList(track.ArtistID);
                return View(track);
            }

            await _trackRepo.UpdateAsync(track);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tracks/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var track = await _trackRepo.GetByIdAsync(id);
            if (track == null) return NotFound();
            return View(track);
        }

        // POST: /Tracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _trackRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
