using Microsoft.AspNetCore.Mvc;
using MusicPlaylistOrganizer.Repositories;

namespace MusicPlaylistOrganizer.Controllers
{
    public class TracksController : Controller
    {
        private readonly ITrackRepository _trackRepo;

        public TracksController(ITrackRepository trackRepo)
        {
            _trackRepo = trackRepo;
        }

        public async Task<IActionResult> Index()
        {
            var tracks = await _trackRepo.GetAllAsync();
            return View(tracks);
        }
    }
}
