using Microsoft.AspNetCore.Mvc;
using MusicPlaylistOrganizer.Models;
using MusicPlaylistOrganizer.Repositories;

namespace MusicPlaylistOrganizer.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly IArtistRepository _artistRepo;

        public ArtistsController(IArtistRepository artistRepo)
        {
            _artistRepo = artistRepo;
        }


        // GET: /Artists
        public async Task<IActionResult> Index(string? sortOrder, string? searchString, string? countryFilter)
        {
            // sort route values for column headers
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CountrySortParm"] = sortOrder == "country" ? "country_desc" : "country";

            // keep filters/search in ViewData so UI keeps values
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCountryFilter"] = countryFilter;

            var artists = await _artistRepo.GetAllAsync(); // includes Tracks

            // 🔍 SEARCH by name or country
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.ToLower();
                artists = artists
                    .Where(a =>
                        a.Name.ToLower().Contains(term) ||
                        (!string.IsNullOrEmpty(a.Country) && a.Country!.ToLower().Contains(term)))
                    .ToList();
            }

            // 🎛 FILTER by country (exact match)
            if (!string.IsNullOrWhiteSpace(countryFilter) && countryFilter != "ALL")
            {
                artists = artists
                    .Where(a => a.Country != null && a.Country == countryFilter)
                    .ToList();
            }

            // 🔽 SORT
            artists = sortOrder switch
            {
                "name_desc" => artists.OrderByDescending(a => a.Name).ToList(),

                "country" => artists
                    .OrderBy(a => a.Country ?? "")
                    .ThenBy(a => a.Name)
                    .ToList(),

                "country_desc" => artists
                    .OrderByDescending(a => a.Country ?? "")
                    .ThenBy(a => a.Name)
                    .ToList(),

                _ => artists.OrderBy(a => a.Name).ToList()
            };

            // build country list for dropdown
            var countries = artists
                .Where(a => !string.IsNullOrEmpty(a.Country))
                .Select(a => a.Country!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            ViewBag.CountryOptions = countries;

            return View(artists);
        }

        // GET: /Artists/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var artist = await _artistRepo.GetByIdAsync(id);
            if (artist == null) return NotFound();
            return View(artist);
        }

        // GET: /Artists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Artists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Country,ApiSourceId,ArtworkUrl")] Artist artist)
        {
            if (!ModelState.IsValid)
            {
                return View(artist);
            }

            await _artistRepo.AddAsync(artist);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Artists/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var artist = await _artistRepo.GetByIdAsync(id);
            if (artist == null) return NotFound();
            return View(artist);
        }

        // POST: /Artists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArtistID,Name,Country,ApiSourceId,ArtworkUrl")] Artist artist)
        {
            if (id != artist.ArtistID)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(artist);
            }

            await _artistRepo.UpdateAsync(artist);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Artists/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var artist = await _artistRepo.GetByIdAsync(id);
            if (artist == null) return NotFound();

            return View(artist);
        }

        // POST: /Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _artistRepo.GetByIdAsync(id);
            if (artist == null) return NotFound();

            // 🚫 Safety check: artist has tracks → don’t delete
            if (artist.Tracks != null && artist.Tracks.Any())
            {
                // Option A: just redisplay the view with an error
                ModelState.AddModelError(string.Empty,
                    "This artist cannot be deleted because they still have tracks in the system. " +
                    "Remove or reassign those tracks first.");

                return View("Delete", artist);
            }

            await _artistRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}