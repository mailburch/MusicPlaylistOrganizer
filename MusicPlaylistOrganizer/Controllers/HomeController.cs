using Microsoft.AspNetCore.Mvc;
using MusicPlaylistOrganizer.Models;
using MusicPlaylistOrganizer.Repositories;

namespace MusicPlaylistOrganizer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPlaylistRepository _playlistRepo;

        public HomeController(IPlaylistRepository playlistRepo)
        {
            _playlistRepo = playlistRepo;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeIndexViewModel
            {
                ArtistCount = await _playlistRepo.GetArtistCountAsync(),
                TrackCount = await _playlistRepo.GetTrackCountAsync(),
                PlaylistCount = await _playlistRepo.GetPlaylistCountAsync()
            };

            return View(vm);
        }

        public IActionResult Privacy() => View();

        public IActionResult Contact() => View();
    }
}
