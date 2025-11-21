using MusicPlaylistOrganizer.Models.Api;

namespace MusicPlaylistOrganizer.Services
{
    public class ItunesMusicCatalogService : IMusicCatalogService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public ItunesMusicCatalogService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["MusicApi:BaseUrl"] ?? "https://itunes.apple.com";
        }

        public async Task<List<ArtistApiResult>> SearchArtistsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new();

            // 1) Base artist search (no artwork yet)
            string url = $"{_baseUrl}/search?term={Uri.EscapeDataString(query)}&entity=musicArtist&limit=10";

            var response = await _http.GetFromJsonAsync<ItunesSearchResponse<ArtistApiResult>>(url);
            var artists = response?.Results ?? new();

            // 2) For each artist, do a lookup for one song to get artworkUrl100
            foreach (var artist in artists)
            {
                if (artist.ArtistId <= 0)
                    continue;

                try
                {
                    string lookupUrl = $"{_baseUrl}/lookup?id={artist.ArtistId}&entity=song&limit=1";

                    var trackResponse = await _http.GetFromJsonAsync<ItunesSearchResponse<TrackApiResult>>(lookupUrl);
                    var firstTrackWithArt = trackResponse?.Results?
                        .FirstOrDefault(t => !string.IsNullOrEmpty(t.ArtworkUrl100));

                    if (firstTrackWithArt != null)
                    {
                        artist.ArtworkUrl100 = firstTrackWithArt.ArtworkUrl100;
                    }
                }
                catch
                {
                    // If lookup fails, just skip artwork for this artist
                }
            }

            return artists;
        }

        public async Task<List<TrackApiResult>> SearchTracksAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new();

            string url = $"{_baseUrl}/search?term={Uri.EscapeDataString(query)}&entity=song&limit=10";

            var response = await _http.GetFromJsonAsync<ItunesSearchResponse<TrackApiResult>>(url);
            return response?.Results ?? new();
        }
    }
}
