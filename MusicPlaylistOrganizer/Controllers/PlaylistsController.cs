using Microsoft.AspNetCore.Mvc;
using MusicPlaylistOrganizer.Repositories;

namespace MusicPlaylistOrganizer.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly IPlaylistRepository _playlistRepo;

        public PlaylistsController(IPlaylistRepository playlistRepo)
        {
            _playlistRepo = playlistRepo;
        }

        public async Task<IActionResult> Index()
        {
            var playlists = await _playlistRepo.GetAllAsync();
            return View(playlists);
        }
    }
}
