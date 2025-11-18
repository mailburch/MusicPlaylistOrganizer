using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MusicPlaylistOrganizer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    ArtistID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiSourceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArtworkUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.ArtistID);
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    PlaylistID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.PlaylistID);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    TrackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false),
                    ArtistID = table.Column<int>(type: "int", nullable: false),
                    ApiSourceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArtworkUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.TrackID);
                    table.ForeignKey(
                        name: "FK_Tracks_Artists_ArtistID",
                        column: x => x.ArtistID,
                        principalTable: "Artists",
                        principalColumn: "ArtistID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistTracks",
                columns: table => new
                {
                    PlaylistID = table.Column<int>(type: "int", nullable: false),
                    TrackID = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistTracks", x => new { x.PlaylistID, x.TrackID });
                    table.ForeignKey(
                        name: "FK_PlaylistTracks_Playlists_PlaylistID",
                        column: x => x.PlaylistID,
                        principalTable: "Playlists",
                        principalColumn: "PlaylistID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistTracks_Tracks_TrackID",
                        column: x => x.TrackID,
                        principalTable: "Tracks",
                        principalColumn: "TrackID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "ArtistID", "ApiSourceId", "ArtworkUrl", "Country", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "USA", "Lo-Fi Wizard" },
                    { 2, null, null, "UK", "Neon Nights" }
                });

            migrationBuilder.InsertData(
                table: "Playlists",
                columns: new[] { "PlaylistID", "Description", "Name" },
                values: new object[] { 1, "Lo-fi beats to focus.", "Chill Study Mix" });

            migrationBuilder.InsertData(
                table: "Tracks",
                columns: new[] { "TrackID", "ApiSourceId", "ArtistID", "ArtworkUrl", "DurationSeconds", "Title" },
                values: new object[,]
                {
                    { 1, null, 1, null, 215, "Study Clouds" },
                    { 2, null, 2, null, 198, "Midnight Neon" }
                });

            migrationBuilder.InsertData(
                table: "PlaylistTracks",
                columns: new[] { "PlaylistID", "TrackID", "Position" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 1, 2, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTracks_TrackID",
                table: "PlaylistTracks",
                column: "TrackID");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_ArtistID",
                table: "Tracks",
                column: "ArtistID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistTracks");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "Artists");
        }
    }
}
