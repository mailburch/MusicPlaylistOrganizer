using Microsoft.AspNetCore.Mvc;
using MusicPlaylistOrganizer.Models;
using MusicPlaylistOrganizer.Repositories;


namespace MusicPlaylistOrganizer.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly IPlaylistRepository _playlistRepo;
        private readonly ITrackRepository _trackRepo;

        public PlaylistsController(
            IPlaylistRepository playlistRepo,
            ITrackRepository trackRepo)
        {
            _playlistRepo = playlistRepo;
            _trackRepo = trackRepo;
        }

        // GET: /Playlists
        public async Task<IActionResult> Index(string? sortOrder, string? searchString, string? trackFilter)
        {
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CountSortParm"] = sortOrder == "tracks" ? "tracks_desc" : "tracks";

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentTrackFilter"] = trackFilter;

            var playlists = await _playlistRepo.GetAllAsync(); // includes PlaylistTracks

            // 🔍 SEARCH: name or description
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.ToLower();
                playlists = playlists
                    .Where(p =>
                        p.Name.ToLower().Contains(term) ||
                        (!string.IsNullOrEmpty(p.Description) && p.Description!.ToLower().Contains(term)))
                    .ToList();
            }

            // 🎛 FILTER by whether it has tracks
            // trackFilter: "all" | "empty" | "nonempty"
            trackFilter ??= "all";

            if (trackFilter == "empty")
            {
                playlists = playlists
                    .Where(p => p.PlaylistTracks == null || p.PlaylistTracks.Count == 0)
                    .ToList();
            }
            else if (trackFilter == "nonempty")
            {
                playlists = playlists
                    .Where(p => p.PlaylistTracks != null && p.PlaylistTracks.Count > 0)
                    .ToList();
            }

            // 🔽 SORT
            playlists = sortOrder switch
            {
                "name_desc" => playlists.OrderByDescending(p => p.Name).ToList(),

                "tracks" => playlists
                    .OrderBy(p => p.PlaylistTracks?.Count ?? 0)
                    .ThenBy(p => p.Name)
                    .ToList(),

                "tracks_desc" => playlists
                    .OrderByDescending(p => p.PlaylistTracks?.Count ?? 0)
                    .ThenBy(p => p.Name)
                    .ToList(),

                _ => playlists.OrderBy(p => p.Name).ToList()
            };

            return View(playlists);
        }

        // GET: /Playlists/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var playlist = await _playlistRepo.GetWithTracksAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }
            return View(playlist);
        }

        // GET: /Playlists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Playlists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Playlist playlist)
        {
            if (!ModelState.IsValid)
            {
                return View(playlist);
            }

            await _playlistRepo.AddAsync(playlist);

            // 🔥 After creating, go straight to Edit so user can add songs
            return RedirectToAction(nameof(Edit), new { id = playlist.PlaylistID });
        }

        // GET: /Playlists/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var playlist = await _playlistRepo.GetWithTracksAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            // All tracks (to add existing songs into playlist)
            var allTracks = await _trackRepo.GetAllAsync();
            ViewBag.AllTracks = allTracks;

            return View(playlist);
        }

        // POST: /Playlists/Edit/5 (update playlist name/description)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlaylistID,Name,Description")] Playlist playlist)
        {
            if (id != playlist.PlaylistID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(playlist);
            }

            await _playlistRepo.UpdateAsync(playlist);
            return RedirectToAction(nameof(Edit), new { id = playlist.PlaylistID });
       }
        // GET: /Playlists/EditPlaylist/5
        public async Task<IActionResult> EditPlaylist(int id)
        {
            var playlist = await _playlistRepo.GetWithTracksAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            // All tracks (to add existing songs into playlist)
            var allTracks = await _trackRepo.GetAllAsync();
            ViewBag.AllTracks = allTracks;

            return View(playlist);
        }

        // POST: /Playlists/EditPlaylist/5 (update playlist name/description)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlaylist(int id, [Bind("PlaylistID,Name,Description")] Playlist playlist)
        {
            if (id != playlist.PlaylistID)
                return BadRequest();

            // If validation fails, return the view to show errors
            if (!ModelState.IsValid)
            {
                // You may need to reload ViewBag.AllTracks here if you use it for error display
                var allTracks = await _trackRepo.GetAllAsync();
                ViewBag.AllTracks = allTracks;
                return View(playlist);
            }

            await _playlistRepo.UpdateAsync(playlist);

            // ⭐ CORRECTED LINE: Redirect to the Index action after successfully saving info
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderTracks([FromBody] ReorderTracksViewModel model)
        {
            if (model == null || model.TrackIds == null || model.TrackIds.Count == 0)
                return BadRequest();

            await _playlistRepo.UpdateTrackOrderAsync(model.PlaylistId, model.TrackIds);

            return Json(new { success = true });
        }

        // POST: /Playlists/AddTrack
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTrack(int playlistId, int trackId)
        {
            await _playlistRepo.AddTrackToPlaylistAsync(playlistId, trackId);
            return RedirectToAction(nameof(Edit), new { id = playlistId });
        }

        // POST: /Playlists/RemoveTrack
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTrack(int playlistId, int trackId)
        {
            await _playlistRepo.RemoveTrackFromPlaylistAsync(playlistId, trackId);
            return RedirectToAction(nameof(Edit), new { id = playlistId });
        }

        // GET: /Playlists/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var playlist = await _playlistRepo.GetWithTracksAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }
            return View(playlist);
        }

        // POST: /Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _playlistRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
