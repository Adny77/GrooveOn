using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GrooveOn.Services.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalArtistId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Biography = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalGenreId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalAlbumId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtistId = table.Column<int>(type: "int", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CoverUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionPlanId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentAmount = table.Column<float>(type: "real", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_SubscriptionPlans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalTrackId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArtistId = table.Column<int>(type: "int", nullable: false),
                    AlbumId = table.Column<int>(type: "int", nullable: true),
                    GenreId = table.Column<int>(type: "int", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false),
                    PreviewUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Songs_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Songs_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Songs_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListeningHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false),
                    PlayedSeconds = table.Column<int>(type: "int", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListeningHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListeningHistories_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListeningHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistSongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlaylistId = table.Column<int>(type: "int", nullable: false),
                    SongId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistSongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaylistSongs_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistSongs_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Administrator sistema", true, "Admin" },
                    { 2, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Standardni korisnik aplikacije", true, "Korisnik" }
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "Id", "Description", "DurationDays", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Osnovni plan sa preview pristupom", 0, true, "Free", 0f },
                    { 2, "Premium plan za 30 dana", 30, true, "Premium", 4.99f },
                    { 3, "Napredni plan za 30 dana", 30, true, "Premium Plus", 9.99f }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Email", "FirstName", "IsActive", "JoinDate", "LastLogin", "LastName", "Password", "PasswordHash", "PasswordSalt", "PhoneNumber", "UserImage", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@grooveon.com", "", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "", "N5j2eDQUE5RjvDf3U6+4bfx3NWw9ul/FoSXjpV+rIXea3HUd6VD+YOgoxlgYM+3E1vXnfcrH8fReRDpTDQmhqA==", "m5bvolwO3KB9dHeODWBZHr5HYlhh5zfTvtj3hRmI463gqewdXgfeTc958AMHq7sPJSA7UeROTP7LRMDNse4O8ZkZ70mtrAbou/aX1K+BuM6CZPDR0M3WSPUnEvCMquxkHr7FVD1KzJDtyacRTWjgXaJmYA4ZEO4Fx9RCqkdZqPc=", "061111111", null, "admin123" },
                    { 2, new DateTime(1998, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin2@grooveon.com", "", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "", "N5j2eDQUE5RjvDf3U6+4bfx3NWw9ul/FoSXjpV+rIXea3HUd6VD+YOgoxlgYM+3E1vXnfcrH8fReRDpTDQmhqA==", "m5bvolwO3KB9dHeODWBZHr5HYlhh5zfTvtj3hRmI463gqewdXgfeTc958AMHq7sPJSA7UeROTP7LRMDNse4O8ZkZ70mtrAbou/aX1K+BuM6CZPDR0M3WSPUnEvCMquxkHr7FVD1KzJDtyacRTWjgXaJmYA4ZEO4Fx9RCqkdZqPc=", "061111112", null, "admin456" },
                    { 3, new DateTime(2002, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "user1@grooveon.com", "", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "", "+ey4BLLFZcx8KGhwLSWmKDwMStixJDVDhtgX2DmA/N0UUd35BCzjj8/nlNMrJimHQJ4CT5RkT9qPf/Qmz0hIZw==", "GX/ecg+UvvKwUEcZF9D3YxzIuU4SsSJ7CTqp4ZzOVNr9oem5SeKztYyzbJOA/if6Arh2/9pMtApP6WY/XjJKag5QfnZdM/cMfU5G4DCfzGjXYWDmSvBDuRILrrJdTqaFtgnP8U7CRdhYm+tK36OIbhJ1qR+q3Al3QoD1m4hxGD4=", "061111113", null, "user1234" },
                    { 4, new DateTime(1999, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "user2@grooveon.com", "", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "", "+ey4BLLFZcx8KGhwLSWmKDwMStixJDVDhtgX2DmA/N0UUd35BCzjj8/nlNMrJimHQJ4CT5RkT9qPf/Qmz0hIZw==", "GX/ecg+UvvKwUEcZF9D3YxzIuU4SsSJ7CTqp4ZzOVNr9oem5SeKztYyzbJOA/if6Arh2/9pMtApP6WY/XjJKag5QfnZdM/cMfU5G4DCfzGjXYWDmSvBDuRILrrJdTqaFtgnP8U7CRdhYm+tK36OIbhJ1qR+q3Al3QoD1m4hxGD4=", "061111114", null, "music2026" },
                    { 5, new DateTime(2001, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "user3@grooveon.com", "", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "", "+ey4BLLFZcx8KGhwLSWmKDwMStixJDVDhtgX2DmA/N0UUd35BCzjj8/nlNMrJimHQJ4CT5RkT9qPf/Qmz0hIZw==", "GX/ecg+UvvKwUEcZF9D3YxzIuU4SsSJ7CTqp4ZzOVNr9oem5SeKztYyzbJOA/if6Arh2/9pMtApP6WY/XjJKag5QfnZdM/cMfU5G4DCfzGjXYWDmSvBDuRILrrJdTqaFtgnP8U7CRdhYm+tK36OIbhJ1qR+q3Al3QoD1m4hxGD4=", "061111115", null, "groove789" },
                    { 6, new DateTime(2003, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "user4@grooveon.com", "", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "", "+ey4BLLFZcx8KGhwLSWmKDwMStixJDVDhtgX2DmA/N0UUd35BCzjj8/nlNMrJimHQJ4CT5RkT9qPf/Qmz0hIZw==", "GX/ecg+UvvKwUEcZF9D3YxzIuU4SsSJ7CTqp4ZzOVNr9oem5SeKztYyzbJOA/if6Arh2/9pMtApP6WY/XjJKag5QfnZdM/cMfU5G4DCfzGjXYWDmSvBDuRILrrJdTqaFtgnP8U7CRdhYm+tK36OIbhJ1qR+q3Al3QoD1m4hxGD4=", "061111116", null, "playlist1" },
                    { 7, new DateTime(1997, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "user5@grooveon.com", "", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "", "+ey4BLLFZcx8KGhwLSWmKDwMStixJDVDhtgX2DmA/N0UUd35BCzjj8/nlNMrJimHQJ4CT5RkT9qPf/Qmz0hIZw==", "GX/ecg+UvvKwUEcZF9D3YxzIuU4SsSJ7CTqp4ZzOVNr9oem5SeKztYyzbJOA/if6Arh2/9pMtApP6WY/XjJKag5QfnZdM/cMfU5G4DCfzGjXYWDmSvBDuRILrrJdTqaFtgnP8U7CRdhYm+tK36OIbhJ1qR+q3Al3QoD1m4hxGD4=", "061111117", null, "album2025" },
                    { 8, new DateTime(1996, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "user6@grooveon.com", "", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "", "+ey4BLLFZcx8KGhwLSWmKDwMStixJDVDhtgX2DmA/N0UUd35BCzjj8/nlNMrJimHQJ4CT5RkT9qPf/Qmz0hIZw==", "GX/ecg+UvvKwUEcZF9D3YxzIuU4SsSJ7CTqp4ZzOVNr9oem5SeKztYyzbJOA/if6Arh2/9pMtApP6WY/XjJKag5QfnZdM/cMfU5G4DCfzGjXYWDmSvBDuRILrrJdTqaFtgnP8U7CRdhYm+tK36OIbhJ1qR+q3Al3QoD1m4hxGD4=", "061111118", null, "rockstar9" },
                    { 9, new DateTime(2004, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "user7@grooveon.com", "", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "", "+ey4BLLFZcx8KGhwLSWmKDwMStixJDVDhtgX2DmA/N0UUd35BCzjj8/nlNMrJimHQJ4CT5RkT9qPf/Qmz0hIZw==", "GX/ecg+UvvKwUEcZF9D3YxzIuU4SsSJ7CTqp4ZzOVNr9oem5SeKztYyzbJOA/if6Arh2/9pMtApP6WY/XjJKag5QfnZdM/cMfU5G4DCfzGjXYWDmSvBDuRILrrJdTqaFtgnP8U7CRdhYm+tK36OIbhJ1qR+q3Al3QoD1m4hxGD4=", "061111119", null, "indie2024" },
                    { 10, new DateTime(2000, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "user8@grooveon.com", "", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "", "", "+ey4BLLFZcx8KGhwLSWmKDwMStixJDVDhtgX2DmA/N0UUd35BCzjj8/nlNMrJimHQJ4CT5RkT9qPf/Qmz0hIZw==", "GX/ecg+UvvKwUEcZF9D3YxzIuU4SsSJ7CTqp4ZzOVNr9oem5SeKztYyzbJOA/if6Arh2/9pMtApP6WY/XjJKag5QfnZdM/cMfU5G4DCfzGjXYWDmSvBDuRILrrJdTqaFtgnP8U7CRdhYm+tK36OIbhJ1qR+q3Al3QoD1m4hxGD4=", "061111120", null, "listener8" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "DateAssigned", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2 },
                    { 3, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3 },
                    { 4, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 4 },
                    { 5, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 5 },
                    { 6, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 6 },
                    { 7, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 7 },
                    { 8, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 8 },
                    { 9, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 9 },
                    { 10, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Albums_ArtistId",
                table: "Albums",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_SongId",
                table: "Favorites",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningHistories_SongId",
                table: "ListeningHistories",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningHistories_UserId",
                table: "ListeningHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_UserId",
                table: "Playlists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistSongs_PlaylistId",
                table: "PlaylistSongs",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistSongs_SongId",
                table: "PlaylistSongs",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_UserId",
                table: "Questions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_AlbumId",
                table: "Songs",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_ArtistId",
                table: "Songs",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_GenreId",
                table: "Songs",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriptionPlanId",
                table: "Subscriptions",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "ListeningHistories");

            migrationBuilder.DropTable(
                name: "PlaylistSongs");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Artists");
        }
    }
}
