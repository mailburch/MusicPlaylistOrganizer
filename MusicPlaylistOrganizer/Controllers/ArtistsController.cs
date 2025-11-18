using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            var artists = await _artistRepo.GetAllAsync();
            return View(artists);
        }

        public async Task<IActionResult> Details(int id)
        {
            var artist = await _artistRepo.GetByIdAsync(id);
            if (artist == null) return NotFound();
            return View(artist);
        }
    }
}
