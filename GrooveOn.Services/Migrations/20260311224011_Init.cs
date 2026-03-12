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
                    { 1, "Osnovni plan sa preview pristupom", 0, true, "Basic account", 0f },
                    { 2, "Premium plan za 30 dana", 30, true, "Premium account", 4.99f }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Email", "FirstName", "IsActive", "JoinDate", "LastLogin", "LastName", "Password", "PasswordHash", "PasswordSalt", "PhoneNumber", "UserImage", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2000, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@grooveon.com", "Marko", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Petrović", "", "1DXaGE27umECSqcLfDX9q3MC1pKiywnEe6OrVkQ8uetWuEOYwNGSn/ANf8i63mhRB9Zj4tNY0siTHaYTJAucVA==", "w2G4ej6MRXJPIoZQMCFX7Hblp0hG4lRcBhNXjdCV1qemT5X63kdNfMBBOO00dtlYTiT8Hzfjw8GBnteRteE0Ch69TifPIqgXZiQS18CC2s+C+zO2IG0yNtfPhRoxfvUMt/Cp5s8uU1z4GqHgMLvBg70y89j4HipWWYbCx0b1rvA=", "061111111", null, "markopetrovic01" },
                    { 2, new DateTime(1998, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin2@grooveon.com", "Nikola", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Jovanović", "", "1DXaGE27umECSqcLfDX9q3MC1pKiywnEe6OrVkQ8uetWuEOYwNGSn/ANf8i63mhRB9Zj4tNY0siTHaYTJAucVA==", "w2G4ej6MRXJPIoZQMCFX7Hblp0hG4lRcBhNXjdCV1qemT5X63kdNfMBBOO00dtlYTiT8Hzfjw8GBnteRteE0Ch69TifPIqgXZiQS18CC2s+C+zO2IG0yNtfPhRoxfvUMt/Cp5s8uU1z4GqHgMLvBg70y89j4HipWWYbCx0b1rvA=", "061111112", null, "nikolajovanovic02" },
                    { 3, new DateTime(2002, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "user1@grooveon.com", "Amar", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Hadžić", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061111113", null, "amarhadzic03" },
                    { 4, new DateTime(1999, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "user2@grooveon.com", "Lejla", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Kovačević", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061111114", null, "lejlakovacevic04" },
                    { 5, new DateTime(2001, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "user3@grooveon.com", "Benjamin", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Mehić", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061111115", null, "benjaminmehic05" },
                    { 6, new DateTime(2003, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "user4@grooveon.com", "Sara", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Delić", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061111116", null, "saradelic06" },
                    { 7, new DateTime(1997, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "user5@grooveon.com", "Adnan", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Karić", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061111117", null, "adnankaric07" },
                    { 8, new DateTime(1996, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "user6@grooveon.com", "Emina", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Selimović", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061111118", null, "eminaselimovic08" },
                    { 9, new DateTime(2004, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "user7@grooveon.com", "Haris", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Mujić", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061111119", null, "harismujic09" },
                    { 10, new DateTime(2000, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "user8@grooveon.com", "Jasmin", true, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Alić", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061111120", null, "jasminalic10" },
                    { 11, new DateTime(2002, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "user11@grooveon.com", "User11", true, new DateTime(2025, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test11", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061950320", null, "user11test11" },
                    { 12, new DateTime(2004, 12, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "user12@grooveon.com", "User12", true, new DateTime(2025, 10, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test12", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061121685", null, "user12test12" },
                    { 13, new DateTime(1996, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "user13@grooveon.com", "User13", true, new DateTime(2026, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test13", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061319621", null, "user13test13" },
                    { 14, new DateTime(2004, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "user14@grooveon.com", "User14", true, new DateTime(2026, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test14", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061684237", null, "user14test14" },
                    { 15, new DateTime(2000, 12, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "user15@grooveon.com", "User15", true, new DateTime(2026, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test15", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061235712", null, "user15test15" },
                    { 16, new DateTime(1997, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "user16@grooveon.com", "User16", true, new DateTime(2025, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test16", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061162127", null, "user16test16" },
                    { 17, new DateTime(2002, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "user17@grooveon.com", "User17", true, new DateTime(2025, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test17", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061170828", null, "user17test17" },
                    { 18, new DateTime(1996, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "user18@grooveon.com", "User18", true, new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test18", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061483253", null, "user18test18" },
                    { 19, new DateTime(1998, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "user19@grooveon.com", "User19", true, new DateTime(2025, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test19", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061747125", null, "user19test19" },
                    { 20, new DateTime(1995, 4, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "user20@grooveon.com", "User20", true, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test20", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061293496", null, "user20test20" },
                    { 21, new DateTime(1997, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "user21@grooveon.com", "User21", true, new DateTime(2025, 12, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test21", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061522332", null, "user21test21" },
                    { 22, new DateTime(2004, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "user22@grooveon.com", "User22", true, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test22", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061967803", null, "user22test22" },
                    { 23, new DateTime(2003, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "user23@grooveon.com", "User23", true, new DateTime(2025, 12, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test23", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061331595", null, "user23test23" },
                    { 24, new DateTime(1999, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "user24@grooveon.com", "User24", true, new DateTime(2026, 8, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test24", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061300470", null, "user24test24" },
                    { 25, new DateTime(1996, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "user25@grooveon.com", "User25", true, new DateTime(2026, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test25", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061782671", null, "user25test25" },
                    { 26, new DateTime(2000, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "user26@grooveon.com", "User26", true, new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test26", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061334567", null, "user26test26" },
                    { 27, new DateTime(1997, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "user27@grooveon.com", "User27", true, new DateTime(2025, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test27", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061642382", null, "user27test27" },
                    { 28, new DateTime(1995, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "user28@grooveon.com", "User28", true, new DateTime(2025, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test28", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061173168", null, "user28test28" },
                    { 29, new DateTime(2002, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "user29@grooveon.com", "User29", true, new DateTime(2025, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test29", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061967862", null, "user29test29" },
                    { 30, new DateTime(2001, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "user30@grooveon.com", "User30", true, new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test30", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061830455", null, "user30test30" },
                    { 31, new DateTime(2004, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "user31@grooveon.com", "User31", true, new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test31", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061737824", null, "user31test31" },
                    { 32, new DateTime(1995, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "user32@grooveon.com", "User32", true, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test32", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061809434", null, "user32test32" },
                    { 33, new DateTime(1998, 11, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "user33@grooveon.com", "User33", true, new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test33", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061242692", null, "user33test33" },
                    { 34, new DateTime(1999, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "user34@grooveon.com", "User34", true, new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test34", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061357265", null, "user34test34" },
                    { 35, new DateTime(1995, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "user35@grooveon.com", "User35", true, new DateTime(2025, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test35", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061471503", null, "user35test35" },
                    { 36, new DateTime(1998, 12, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "user36@grooveon.com", "User36", true, new DateTime(2025, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test36", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061459797", null, "user36test36" },
                    { 37, new DateTime(1997, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "user37@grooveon.com", "User37", true, new DateTime(2025, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test37", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061453120", null, "user37test37" },
                    { 38, new DateTime(2003, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "user38@grooveon.com", "User38", true, new DateTime(2026, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test38", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061615801", null, "user38test38" },
                    { 39, new DateTime(1995, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "user39@grooveon.com", "User39", true, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test39", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061758173", null, "user39test39" },
                    { 40, new DateTime(1995, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "user40@grooveon.com", "User40", true, new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test40", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061196294", null, "user40test40" },
                    { 41, new DateTime(2004, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "user41@grooveon.com", "User41", true, new DateTime(2026, 11, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test41", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061773707", null, "user41test41" },
                    { 42, new DateTime(1995, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "user42@grooveon.com", "User42", true, new DateTime(2026, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test42", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061511092", null, "user42test42" },
                    { 43, new DateTime(1995, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "user43@grooveon.com", "User43", true, new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test43", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061327036", null, "user43test43" },
                    { 44, new DateTime(1997, 12, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "user44@grooveon.com", "User44", true, new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test44", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061516561", null, "user44test44" },
                    { 45, new DateTime(2002, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "user45@grooveon.com", "User45", true, new DateTime(2026, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test45", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061177016", null, "user45test45" },
                    { 46, new DateTime(1997, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "user46@grooveon.com", "User46", true, new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test46", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061494683", null, "user46test46" },
                    { 47, new DateTime(2000, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "user47@grooveon.com", "User47", true, new DateTime(2025, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test47", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061343435", null, "user47test47" },
                    { 48, new DateTime(2001, 7, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "user48@grooveon.com", "User48", true, new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test48", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061338309", null, "user48test48" },
                    { 49, new DateTime(1998, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "user49@grooveon.com", "User49", true, new DateTime(2025, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test49", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061844014", null, "user49test49" },
                    { 50, new DateTime(1998, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "user50@grooveon.com", "User50", true, new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test50", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061782063", null, "user50test50" },
                    { 51, new DateTime(1997, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "user51@grooveon.com", "User51", true, new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test51", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061273133", null, "user51test51" },
                    { 52, new DateTime(1999, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "user52@grooveon.com", "User52", true, new DateTime(2025, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test52", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061737805", null, "user52test52" },
                    { 53, new DateTime(1997, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "user53@grooveon.com", "User53", true, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test53", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061820570", null, "user53test53" },
                    { 54, new DateTime(2003, 7, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "user54@grooveon.com", "User54", true, new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test54", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061621458", null, "user54test54" },
                    { 55, new DateTime(2004, 11, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "user55@grooveon.com", "User55", true, new DateTime(2025, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test55", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061308464", null, "user55test55" },
                    { 56, new DateTime(2000, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "user56@grooveon.com", "User56", true, new DateTime(2025, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test56", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061756269", null, "user56test56" },
                    { 57, new DateTime(2001, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "user57@grooveon.com", "User57", true, new DateTime(2025, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test57", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061981830", null, "user57test57" },
                    { 58, new DateTime(1996, 7, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "user58@grooveon.com", "User58", true, new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test58", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061351703", null, "user58test58" },
                    { 59, new DateTime(1999, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "user59@grooveon.com", "User59", true, new DateTime(2025, 12, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test59", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061273295", null, "user59test59" },
                    { 60, new DateTime(1999, 9, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "user60@grooveon.com", "User60", true, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test60", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061269736", null, "user60test60" },
                    { 61, new DateTime(1998, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "user61@grooveon.com", "User61", true, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test61", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061481607", null, "user61test61" },
                    { 62, new DateTime(1999, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "user62@grooveon.com", "User62", true, new DateTime(2025, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test62", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061434631", null, "user62test62" },
                    { 63, new DateTime(1998, 7, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "user63@grooveon.com", "User63", true, new DateTime(2026, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test63", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061375476", null, "user63test63" },
                    { 64, new DateTime(2003, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "user64@grooveon.com", "User64", true, new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test64", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061666983", null, "user64test64" },
                    { 65, new DateTime(1999, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "user65@grooveon.com", "User65", true, new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test65", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061944278", null, "user65test65" },
                    { 66, new DateTime(2003, 11, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "user66@grooveon.com", "User66", true, new DateTime(2026, 4, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test66", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061121186", null, "user66test66" },
                    { 67, new DateTime(2002, 1, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "user67@grooveon.com", "User67", true, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test67", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061235289", null, "user67test67" },
                    { 68, new DateTime(1996, 7, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "user68@grooveon.com", "User68", true, new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test68", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061665739", null, "user68test68" },
                    { 69, new DateTime(1997, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "user69@grooveon.com", "User69", true, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test69", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061625316", null, "user69test69" },
                    { 70, new DateTime(2003, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "user70@grooveon.com", "User70", true, new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Test70", "", "CXK5ErFNypwJVVCQDn1zC4+v/O0y1KByt6QLGOF73CDLAJRRkEG9El6HQONaRvjOIf6jONHgTAwAKHvVYph5qg==", "UoZ2qdpnYWYNJybRcEa4o4zbYu3+siJfTyOhzdKDZqkIuYQO+Q5WtDVTzK9s40ls0828T1zU10IhOs8aqR6Uj4EW32iS126EdCrlgzVILknM8VcQouDorN3IFoRoy29QQKlhWBudteIaX7dPVGLmsbhUE9gY2SZ6HOvsRGKpv5E=", "061988344", null, "user70test70" }
                });

            migrationBuilder.InsertData(
                table: "Subscriptions",
                columns: new[] { "Id", "ExpiryDate", "IsActive", "PaymentAmount", "PaymentDate", "PaymentMethod", "StartDate", "SubscriptionPlanId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 4.99f, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Card", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 },
                    { 2, new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 9.99f, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "PayPal", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2 },
                    { 3, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3 },
                    { 4, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4 },
                    { 5, new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 4.99f, new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Card", new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 5 },
                    { 6, new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 6 },
                    { 7, new DateTime(2026, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 9.99f, new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "PayPal", new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 7 },
                    { 8, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 8 },
                    { 9, new DateTime(2026, 4, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 4.99f, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Card", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 9 },
                    { 10, new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10 },
                    { 11, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 4.99f, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Card", new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3 },
                    { 12, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4 },
                    { 13, new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 9.99f, new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "PayPal", new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 5 },
                    { 14, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 6 },
                    { 15, new DateTime(2026, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 4.99f, new DateTime(2026, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Card", new DateTime(2026, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 7 },
                    { 16, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 8 },
                    { 17, new DateTime(2026, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 9.99f, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "PayPal", new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 9 },
                    { 18, new DateTime(2026, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10 },
                    { 19, new DateTime(2026, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 4.99f, new DateTime(2026, 8, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Card", new DateTime(2026, 8, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 },
                    { 20, new DateTime(2026, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2 },
                    { 21, new DateTime(2026, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 4.99f, new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Card", new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3 },
                    { 22, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4 },
                    { 23, new DateTime(2026, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 9.99f, new DateTime(2026, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "PayPal", new DateTime(2026, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 5 },
                    { 24, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 6 },
                    { 25, new DateTime(2026, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 4.99f, new DateTime(2026, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Card", new DateTime(2026, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 7 },
                    { 26, new DateTime(2026, 11, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 11, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 8 },
                    { 27, new DateTime(2027, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 9.99f, new DateTime(2026, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "PayPal", new DateTime(2026, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 9 },
                    { 28, new DateTime(2026, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 0f, null, null, new DateTime(2026, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10 }
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
