using Microsoft.AspNetCore.Mvc;
using MusicPlaylistOrganizer.Models;
using MusicPlaylistOrganizer.Repositories;

namespace MusicPlaylistOrganizer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IArtistRepository _artistRepo;
        private readonly ITrackRepository _trackRepo;
        private readonly IPlaylistRepository _playlistRepo;

        public HomeController(
            IArtistRepository artistRepo,
            ITrackRepository trackRepo,
            IPlaylistRepository playlistRepo)
        {
            _artistRepo = artistRepo;
            _trackRepo = trackRepo;
            _playlistRepo = playlistRepo;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeIndexViewModel
            {
                ArtistCount = (await _artistRepo.GetAllAsync()).Count,
                TrackCount = (await _trackRepo.GetAllAsync()).Count,
                PlaylistCount = (await _playlistRepo.GetAllAsync()).Count
            };

            return View(vm);
        }

        public IActionResult Privacy() => View();
        public IActionResult Contact() => View();
    }
}
