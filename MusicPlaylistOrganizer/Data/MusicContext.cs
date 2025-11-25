using Microsoft.EntityFrameworkCore;
using MusicPlaylistOrganizer.Models;

namespace MusicPlaylistOrganizer.Data
{
    public class MusicContext : DbContext
    {
        public MusicContext(DbContextOptions<MusicContext> options) : base(options) { }

        public DbSet<Artist> Artists => Set<Artist>();
        public DbSet<Track> Tracks => Set<Track>();
        public DbSet<Playlist> Playlists => Set<Playlist>();
        public DbSet<PlaylistTrack> PlaylistTracks => Set<PlaylistTrack>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlaylistTrack>()
                .HasKey(pt => new { pt.PlaylistID, pt.TrackID });

            // Seed Artists
            modelBuilder.Entity<Artist>().HasData(
                new Artist { ArtistID = 1, Name = "Lo-Fi Wizard", Genre = "USA" },
                new Artist { ArtistID = 2, Name = "Neon Nights", Genre = "UK" }
            );

            // Seed Tracks
            modelBuilder.Entity<Track>().HasData(
                new Track { TrackID = 1, Title = "Study Clouds", DurationSeconds = 215, ArtistID = 1 },
                new Track { TrackID = 2, Title = "Midnight Neon", DurationSeconds = 198, ArtistID = 2 }
            );

            // Seed Playlists
            modelBuilder.Entity<Playlist>().HasData(
                new Playlist { PlaylistID = 1, Name = "Chill Study Mix", Description = "Lo-fi beats to focus." }
            );

            modelBuilder.Entity<PlaylistTrack>().HasData(
                new PlaylistTrack { PlaylistID = 1, TrackID = 1, Position = 1 },
                new PlaylistTrack { PlaylistID = 1, TrackID = 2, Position = 2 }
            );
        }
    }
}
