using GrooveOn.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GrooveOn.Services.Database
{
    public class GrooveOnDbContext : DbContext
    {
        private static readonly string[] FirstNames =
{
    "Amar", "Lejla", "Benjamin", "Sara", "Adnan", "Emina", "Haris", "Jasmin",
    "Tarik", "Amina", "Dino", "Lamija", "Kenan", "Naida", "Emir", "Selma",
    "Faruk", "Mila", "Anes", "Ilma", "Kerim", "Nejra", "Samir", "Hana",
    "Armin", "Ajla", "Nedim", "Dalia", "Ermin", "Nora", "Damir", "Ilda"
};

    private static readonly string[] LastNames =
    {
    "Had�ic", "Kovacevic", "Mehic", "Delic", "Karic", "Selimovic", "Mujic",
    "Alic", "Begic", "Softic", "Hod�ic", "�aric", "Mujanovic", "Imamovic",
    "Pjanic", "Bajric", "Osmanovic", "Halilovic", "Muratovic", "Colic",
    "Spahic", "Zukic", "Velagic", "Brkic", "Demirovic", "Lulic", "Be�ic"
};

    private static string NormalizeForUsername(string value)
    {
        return value
            .ToLower()
            .Replace("c", "c")
            .Replace("c", "c")
            .Replace("d", "d")
            .Replace("�", "s")
            .Replace("�", "z");
    }

    private static string GetLoremPicsumImage(string type, int id)
    {
        return $"https://picsum.photos/seed/grooveon-{type}-{id}/500/500";
    }
        public GrooveOnDbContext(DbContextOptions<GrooveOnDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Song> Songs { get; set; }

        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Player> Players { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<PlayHistory> PlayHistories { get; set; }
        public DbSet<AlbumGenre> AlbumGenres { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Answer>()
            .HasOne(x => x.Question)
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
    .HasIndex(x => new { x.UserId, x.SongId })
    .IsUnique();

            modelBuilder.Entity<Answer>()
                .HasOne(x => x.Admin)
                .WithMany()
                .HasForeignKey(x => x.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlbumGenre>()
    .HasIndex(x => new { x.AlbumId, x.GenreId })
    .IsUnique();

            modelBuilder.Entity<AlbumGenre>()
                .HasOne(x => x.Album)
                .WithMany(x => x.AlbumGenres)
                .HasForeignKey(x => x.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlbumGenre>()
                .HasOne(x => x.Genre)
                .WithMany(x => x.AlbumGenres)
                .HasForeignKey(x => x.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.ExternalGenreId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Source)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(x => new { x.ExternalGenreId, x.Source }).IsUnique();
                entity.HasIndex(x => x.Name);
            });

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "Administrator sistema",
                    CreatedAt = new DateTime(2026, 3, 10),
                    IsActive = true
                },
                new Role
                {
                    Id = 2,
                    Name = "User",
                    Description = "Standard application user",
                    CreatedAt = new DateTime(2026, 3, 10),
                    IsActive = true
                }
            );

            string adminHash = UserHelper.CreatePasswordHash("Admin123!");
            string userHash = UserHelper.CreatePasswordHash("User123!");

            var baseUsers = new List<User>
    {
        new User
        {
            Id = 1,
            FirstName = "Dejan",
            LastName = "Music",
            Username = "dejanmusic01",
            Password = string.Empty,
            PasswordHash = adminHash,
            Email = "testniadminmuzicar@gmail.com",
            UserImage = null,
            DateOfBirth = new DateTime(2000, 1, 15),
            PhoneNumber = "061111111",
            IsActive = true,
            JoinDate = new DateTime(2026, 3, 10),
            LastLogin = null
        },
        new User
        {
            Id = 2,
            FirstName = "Milan",
            LastName = "Kostadinovic",
            Username = "milankostadinovic02",
            Password = string.Empty,
            PasswordHash = adminHash,
            Email = "admin2@grooveon.com",
            UserImage = null,
            DateOfBirth = new DateTime(1998, 6, 20),
            PhoneNumber = "061111112",
            IsActive = true,
            JoinDate = new DateTime(2026, 3, 10),
            LastLogin = null
        },
        new User
        {
            Id = 3,
            FirstName = "Amar",
            LastName = "Hadzic",
            Username = "amarhadzic03",
            Password = string.Empty,
            PasswordHash = userHash,
            Email = "user1@grooveon.com",
            UserImage = null,
            DateOfBirth = new DateTime(2002, 5, 21),
            PhoneNumber = "061111113",
            IsActive = true,
            JoinDate = new DateTime(2025, 1, 10),
            LastLogin = null
        },
        new User
        {
            Id = 4,
            FirstName = "Lejla",
            LastName = "Kovacevic",
            Username = "lejlakovacevic04",
            Password = string.Empty,
            PasswordHash = userHash,
            Email = "user2@grooveon.com",
            UserImage = null,
            DateOfBirth = new DateTime(1999, 9, 10),
            PhoneNumber = "061111114",
            IsActive = true,
            JoinDate = new DateTime(2025, 2, 15),
            LastLogin = null
        },
        new User
        {
            Id = 5,
            FirstName = "Benjamin",
            LastName = "Mehic",
            Username = "benjaminmehic05",
            Password = string.Empty,
            PasswordHash = userHash,
            Email = "user3@grooveon.com",
            UserImage = null,
            DateOfBirth = new DateTime(2001, 3, 14),
            PhoneNumber = "061111115",
            IsActive = true,
            JoinDate = new DateTime(2025, 3, 12),
            LastLogin = null
        },
        new User
        {
            Id = 6,
            FirstName = "Sara",
            LastName = "Delic",
            Username = "saradelic06",
            Password = string.Empty,
            PasswordHash = userHash,
            Email = "user4@grooveon.com",
            UserImage = null,
            DateOfBirth = new DateTime(2003, 7, 8),
            PhoneNumber = "061111116",
            IsActive = true,
            JoinDate = new DateTime(2025, 4, 8),
            LastLogin = null
        },
        new User
        {
            Id = 7,
            FirstName = "Adnan",
            LastName = "Karic",
            Username = "adnankaric07",
            Password = string.Empty,
            PasswordHash = userHash,
            Email = "user5@grooveon.com",
            UserImage = null,
            DateOfBirth = new DateTime(1997, 11, 2),
            PhoneNumber = "061111117",
            IsActive = true,
            JoinDate = new DateTime(2025, 5, 20),
            LastLogin = null
        },
        new User
        {
            Id = 8,
            FirstName = "Emina",
            LastName = "Selimovic",
            Username = "eminaselimovic08",
            Password = string.Empty,
            PasswordHash = userHash,
            Email = "user6@grooveon.com",
            UserImage = null,
            DateOfBirth = new DateTime(1996, 12, 25),
            PhoneNumber = "061111118",
            IsActive = true,
            JoinDate = new DateTime(2026, 1, 5),
            LastLogin = null
        },
        new User
        {
            Id = 9,
            FirstName = "Haris",
            LastName = "Mujic",
            Username = "harismujic09",
            Password = string.Empty,
            PasswordHash = userHash,
            Email = "user7@grooveon.com",
            UserImage = null,
            DateOfBirth = new DateTime(2004, 1, 9),
            PhoneNumber = "061111119",
            IsActive = true,
            JoinDate = new DateTime(2026, 2, 18),
            LastLogin = null
        },
        new User
        {
            Id = 10,
            FirstName = "Jasmin",
            LastName = "Alic",
            Username = "jasminalic10",
            Password = string.Empty,
            PasswordHash = userHash,
            Email = "user8@grooveon.com",
            UserImage = null,
            DateOfBirth = new DateTime(2000, 8, 17),
            PhoneNumber = "061111120",
            IsActive = true,
            JoinDate = new DateTime(2026, 3, 10),
            LastLogin = null
        },

        new User
{
    Id = 11,
    FirstName = "Fahrudin",
    LastName = "Music",
    Username = "fahrudinmusic11",
    Password = string.Empty,
    PasswordHash = userHash,
    Email = "testnimuzicar@gmail.com",
    UserImage = GetLoremPicsumImage("user", 3),
    DateOfBirth = new DateTime(2001, 5, 10),
    PhoneNumber = "061555555",
    IsActive = true,
    JoinDate = new DateTime(2026, 3, 10),
    LastLogin = null
}
    };

            modelBuilder.Entity<Artist>().HasData(
            new Artist { Id = 1, ExternalArtistId = "4050205", Source = "Deezer", Name = "The Weeknd", Picture = "https://cdn-images.dzcdn.net/images/artist/581693b4724a7fcfa754455101e13a44/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 2, ExternalArtistId = "246791", Source = "Deezer", Name = "Drake", Picture = "https://cdn-images.dzcdn.net/images/artist/5d2fa7f140a6bdc2c864c3465a61fc71/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 3, ExternalArtistId = "4495513", Source = "Deezer", Name = "Travis Scott", Picture = "https://cdn-images.dzcdn.net/images/artist/8d8316146026d7e6ce377e314536df62/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 4, ExternalArtistId = "230", Source = "Deezer", Name = "Kanye West", Picture = "https://cdn-images.dzcdn.net/images/artist/bb76c2ee3b068726ab4c37b0aabdb57a/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 5, ExternalArtistId = "92", Source = "Deezer", Name = "Linkin Park", Picture = "https://cdn-images.dzcdn.net/images/artist/4886905210739af3438990897bad3a98/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 6, ExternalArtistId = "134790", Source = "Deezer", Name = "Tame Impala", Picture = "https://cdn-images.dzcdn.net/images/artist/879015e713cc6ad6ffaeec154c027505/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 7, ExternalArtistId = "12246", Source = "Deezer", Name = "Taylor Swift", Picture = "https://cdn-images.dzcdn.net/images/artist/e528e270424103b527f8a27ac625563b/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 8, ExternalArtistId = "210", Source = "Deezer", Name = "Eagles", Picture = "https://cdn-images.dzcdn.net/images/artist/6c9ff651bf4eb2dced0ccf3fb8dbfc61/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 9, ExternalArtistId = "1182", Source = "Deezer", Name = "Arctic Monkeys", Picture = "https://cdn-images.dzcdn.net/images/artist/6c03e4c7c36800897fd468633286db24/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 10, ExternalArtistId = "409796", Source = "Deezer", Name = "Disclosure", Picture = "https://cdn-images.dzcdn.net/images/artist/83aa8d9bb361fe039ee1caf0488eb0c1/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 11, ExternalArtistId = "259", Source = "Deezer", Name = "Michael Jackson", Picture = "https://cdn-images.dzcdn.net/images/artist/97fae13b2b30e4aec2e8c9e0c7839d92/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 12, ExternalArtistId = "75798", Source = "Deezer", Name = "Adele", Picture = "https://cdn-images.dzcdn.net/images/artist/e5fc443d2abc03b487234ba4de65a001/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 13, ExternalArtistId = "1562681", Source = "Deezer", Name = "Ariana Grande", Picture = "https://cdn-images.dzcdn.net/images/artist/5fcde7fde7cde95fc36d4576afcfb49f/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 14, ExternalArtistId = "564", Source = "Deezer", Name = "Rihanna", Picture = "https://cdn-images.dzcdn.net/images/artist/b78cdc205fae2641b89208e78b30e1b3/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 15, ExternalArtistId = "182", Source = "Deezer", Name = "Red Hot Chili Peppers", Picture = "https://cdn-images.dzcdn.net/images/artist/238f5524a401dfdd5cac685f0f7989bd/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 16, ExternalArtistId = "399", Source = "Deezer", Name = "Radiohead", Picture = "https://cdn-images.dzcdn.net/images/artist/96b688020014a21cb80a0268b90287f5/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 17, ExternalArtistId = "864274", Source = "Deezer", Name = "Porter Robinson", Picture = "https://cdn-images.dzcdn.net/images/artist/a83618b66fa207ef0840f167e5ad59c4/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 18, ExternalArtistId = "5313805", Source = "Deezer", Name = "Harry Styles", Picture = "https://cdn-images.dzcdn.net/images/artist/1151dba9b3edc0633adf35b64c21713f/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 19, ExternalArtistId = "13", Source = "Deezer", Name = "Eminem", Picture = "https://cdn-images.dzcdn.net/images/artist/0f30bbd33a680030054af004d698d6ac/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 20, ExternalArtistId = "429675", Source = "Deezer", Name = "Bruno Mars", Picture = "https://cdn-images.dzcdn.net/images/artist/90f0b5b11df4f87ee878f38569b5995b/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) },
            new Artist { Id = 21, ExternalArtistId = "1517560", Source = "Deezer", Name = "Madeon", Picture = "https://cdn-images.dzcdn.net/images/artist/4c44e7101b476c5f312e087a4bc8aec0/250x250-000000-80-0-0.jpg", CreatedAt = new DateTime(2020, 3, 25) }
        );

            modelBuilder.Entity<Album>().HasData(
                new Album { Id = 1, ExternalAlbumId = "137217782", Source = "Deezer", Title = "After Hours", ArtistId = 1, ReleaseDate = new DateTime(2020, 3, 20), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 2, ExternalAlbumId = "13082992", Source = "Deezer", Title = "Views", ArtistId = 2, ReleaseDate = new DateTime(2016, 5, 6), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 3, ExternalAlbumId = "469682765", Source = "Deezer", Title = "UTOPIA", ArtistId = 3, ReleaseDate = new DateTime(2023, 7, 28), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 4, ExternalAlbumId = "8699402", Source = "Deezer", Title = "Graduation", ArtistId = 4, ReleaseDate = new DateTime(2007, 9, 11), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 5, ExternalAlbumId = "1346746", Source = "Deezer", Title = "Meteora", ArtistId = 5, ReleaseDate = new DateTime(2003, 3, 24), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 6, ExternalAlbumId = "10709540", Source = "Deezer", Title = "Currents", ArtistId = 6, ReleaseDate = new DateTime(2015, 7, 17), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 7, ExternalAlbumId = "130721292", Source = "Deezer", Title = "Red", ArtistId = 7, ReleaseDate = new DateTime(2012, 10, 22), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 8, ExternalAlbumId = "6670584", Source = "Deezer", Title = "Hotel California (2013 Remaster)", ArtistId = 8, ReleaseDate = new DateTime(2006, 4, 3), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 9, ExternalAlbumId = "509888701", Source = "Deezer", Title = "Whatever People Say I Am, That's What I'm Not", ArtistId = 9, ReleaseDate = new DateTime(2006, 2, 18), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 10, ExternalAlbumId = "7480190", Source = "Deezer", Title = "Settle (Special Edition)", ArtistId = 10, ReleaseDate = new DateTime(2014, 1, 1), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 11, ExternalAlbumId = "96126", Source = "Deezer", Title = "Thriller", ArtistId = 11, ReleaseDate = new DateTime(1983, 8, 1), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 12, ExternalAlbumId = "5814031", Source = "Deezer", Title = "Bad (2012 Remaster)", ArtistId = 11, ReleaseDate = new DateTime(2012, 9, 17), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 13, ExternalAlbumId = "746059", Source = "Deezer", Title = "21", ArtistId = 12, ReleaseDate = new DateTime(2011, 2, 22), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 14, ExternalAlbumId = "86773062", Source = "Deezer", Title = "thank u, next", ArtistId = 13, ReleaseDate = new DateTime(2019, 2, 8), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 15, ExternalAlbumId = "6120369", Source = "Deezer", Title = "Unapologetic (Deluxe)", ArtistId = 14, ReleaseDate = new DateTime(2012, 11, 19), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 16, ExternalAlbumId = "1238967", Source = "Deezer", Title = "Loud", ArtistId = 14, ReleaseDate = new DateTime(2010, 11, 16), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 17, ExternalAlbumId = "13357219", Source = "Deezer", Title = "The Life Of Pablo", ArtistId = 4, ReleaseDate = new DateTime(2016, 6, 15), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 18, ExternalAlbumId = "85660", Source = "Deezer", Title = "By the Way (Deluxe Edition)", ArtistId = 15, ReleaseDate = new DateTime(2002, 6, 25), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 19, ExternalAlbumId = "82107", Source = "Deezer", Title = "Stadium Arcadium", ArtistId = 15, ReleaseDate = new DateTime(2006, 5, 5), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 20, ExternalAlbumId = "14879699", Source = "Deezer", Title = "OK Computer", ArtistId = 16, ReleaseDate = new DateTime(1997, 6, 17), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 21, ExternalAlbumId = "8198764", Source = "Deezer", Title = "Worlds", ArtistId = 17, ReleaseDate = new DateTime(2014, 8, 13), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 22, ExternalAlbumId = "122664252", Source = "Deezer", Title = "Fine Line", ArtistId = 18, ReleaseDate = new DateTime(2019, 12, 13), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 23, ExternalAlbumId = "430632", Source = "Deezer", Title = "Encore (Deluxe Version)", ArtistId = 19, ReleaseDate = new DateTime(2004, 11, 12), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 24, ExternalAlbumId = "1441212", Source = "Deezer", Title = "Dangerous", ArtistId = 11, ReleaseDate = new DateTime(1991, 11, 21), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 25, ExternalAlbumId = "739505", Source = "Deezer", Title = "Doo-Wops & Hooligans", ArtistId = 20, ReleaseDate = new DateTime(2010, 10, 5), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 26, ExternalAlbumId = "9854424", Source = "Deezer", Title = "Adventure (Deluxe)", ArtistId = 21, ReleaseDate = new DateTime(2015, 3, 30), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 27, ExternalAlbumId = "41373501", Source = "Deezer", Title = "Harry Styles", ArtistId = 18, ReleaseDate = new DateTime(2017, 5, 12), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a41167cfcc7e840821fad5f5f5f91da2/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 28, ExternalAlbumId = "319924157", Source = "Deezer", Title = "Harry's House", ArtistId = 18, ReleaseDate = new DateTime(2022, 5, 20), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) },
                new Album { Id = 29, ExternalAlbumId = "903024552", Source = "Deezer", Title = "Aperture", ArtistId = 21, ReleaseDate = new DateTime(2026, 1, 22), CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fee004942feff253f7bbca63740ab519/250x250-000000-80-0-0.jpg", Description = null, CreatedAt = new DateTime(2020, 3, 25) }
            );

            modelBuilder.Entity<Song>().HasData(
            new Song { Id = 1, ExternalTrackId = "908604532", Source = "Deezer", Title = "Alone Again", ArtistId = 1, AlbumId = 1, DurationSeconds = 252, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/c/6/0/6c6474222c48abd5d908f45cb0ef05b1.mp3?hdnea=exp=1776617511~acl=/api/1/1/6/c/6/0/6c6474222c48abd5d908f45cb0ef05b1.mp3*~data=user_id=0,application_id=42~hmac=f5c34c9ef61d2d38e88acef67547830069ae0ba1b1e5fd6209978415129df1e5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 2, ExternalTrackId = "908604542", Source = "Deezer", Title = "Too Late", ArtistId = 1, AlbumId = 1, DurationSeconds = 239, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/4/8/0/74804470072f7f31b6fae0da3306ae09.mp3?hdnea=exp=1776617512~acl=/api/1/1/7/4/8/0/74804470072f7f31b6fae0da3306ae09.mp3*~data=user_id=0,application_id=42~hmac=a741abf947a117667650ce095392d68a476779da44f26ccc992e215bd3c61613", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 3, ExternalTrackId = "908604552", Source = "Deezer", Title = "Hardest To Love", ArtistId = 1, AlbumId = 1, DurationSeconds = 211, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/7/9/0/a79e231bd1f4202c89e2c79a8c45b4a4.mp3?hdnea=exp=1776617512~acl=/api/1/1/a/7/9/0/a79e231bd1f4202c89e2c79a8c45b4a4.mp3*~data=user_id=0,application_id=42~hmac=0b5df847e1a6365e8cc357077556b253b4a0d24a301cafef661f3faa0f7763fe", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 4, ExternalTrackId = "908604562", Source = "Deezer", Title = "Scared To Live", ArtistId = 1, AlbumId = 1, DurationSeconds = 191, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/4/6/0/84661ede95264106311c08ee96d25d0b.mp3?hdnea=exp=1776617513~acl=/api/1/1/8/4/6/0/84661ede95264106311c08ee96d25d0b.mp3*~data=user_id=0,application_id=42~hmac=86f65ca5a4da40928f416739c409cb931c8421f7543856847f1fb0e33cb094aa", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 5, ExternalTrackId = "908604572", Source = "Deezer", Title = "Snowchild", ArtistId = 1, AlbumId = 1, DurationSeconds = 247, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/0/b/0/b0b959f417d1e568ab9bde40b18f2103.mp3?hdnea=exp=1776617514~acl=/api/1/1/b/0/b/0/b0b959f417d1e568ab9bde40b18f2103.mp3*~data=user_id=0,application_id=42~hmac=a579515491c92420324fc2d88e5ffcb4ee62fa3c5215d2ab4928ab5ebef67b5a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 6, ExternalTrackId = "908604582", Source = "Deezer", Title = "Escape From LA", ArtistId = 1, AlbumId = 1, DurationSeconds = 355, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/f/4/0/0f42452548e051f74b009bba1fef406a.mp3?hdnea=exp=1776617514~acl=/api/1/1/0/f/4/0/0f42452548e051f74b009bba1fef406a.mp3*~data=user_id=0,application_id=42~hmac=9a003b0fdb5eedf3c8525ee45e870dfbd871e1e3c5d0f10e626de99246b08a4e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 7, ExternalTrackId = "908604592", Source = "Deezer", Title = "Heartless", ArtistId = 1, AlbumId = 1, DurationSeconds = 200, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/c/a/0/8cada0830effa568513748e3480c1919.mp3?hdnea=exp=1776617515~acl=/api/1/1/8/c/a/0/8cada0830effa568513748e3480c1919.mp3*~data=user_id=0,application_id=42~hmac=64ada56d5a7ff1fc381539b4d66cae93c7f86e4b1b0d288e2af935523119e61e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 8, ExternalTrackId = "908604602", Source = "Deezer", Title = "Faith", ArtistId = 1, AlbumId = 1, DurationSeconds = 283, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/d/6/0/ad6eeb2225a03411603c41f9cf0645b4.mp3?hdnea=exp=1776617515~acl=/api/1/1/a/d/6/0/ad6eeb2225a03411603c41f9cf0645b4.mp3*~data=user_id=0,application_id=42~hmac=e371de16cc98d9010d60becbe4b3933ebbaea89ee73b2b369867a6f43562651c", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 9, ExternalTrackId = "908604612", Source = "Deezer", Title = "Blinding Lights", ArtistId = 1, AlbumId = 1, DurationSeconds = 200, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/b/2/0/1b27825bf63c36edcdc7fac9f920214e.mp3?hdnea=exp=1776617516~acl=/api/1/1/1/b/2/0/1b27825bf63c36edcdc7fac9f920214e.mp3*~data=user_id=0,application_id=42~hmac=de76cbd6d773f9240ef3ab0c36495848afe86c67de68847f9fb9ec9e6b0c1573", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 10, ExternalTrackId = "908604622", Source = "Deezer", Title = "In Your Eyes", ArtistId = 1, AlbumId = 1, DurationSeconds = 237, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/8/e/0/58ea87a2e8eb244a4d351b832ab8dc4e.mp3?hdnea=exp=1776617517~acl=/api/1/1/5/8/e/0/58ea87a2e8eb244a4d351b832ab8dc4e.mp3*~data=user_id=0,application_id=42~hmac=618ea3aa39f2bf542e954d7b5d5bd8f22f0e1ae7c8c67760aa3f77c1de3a4f7d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 11, ExternalTrackId = "908604632", Source = "Deezer", Title = "Save Your Tears", ArtistId = 1, AlbumId = 1, DurationSeconds = 215, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/0/3/0/6030d100df57535a6dd78ee08bf99261.mp3?hdnea=exp=1776617517~acl=/api/1/1/6/0/3/0/6030d100df57535a6dd78ee08bf99261.mp3*~data=user_id=0,application_id=42~hmac=3b88ade93dc6abeb0054aaf4962ea2f983bbca5cb8ff642524c433db80a2e9c8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 12, ExternalTrackId = "908604642", Source = "Deezer", Title = "Repeat After Me (Interlude)", ArtistId = 1, AlbumId = 1, DurationSeconds = 195, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/b/6/0/5b678e60bda17c1637ec90326906d76a.mp3?hdnea=exp=1776617518~acl=/api/1/1/5/b/6/0/5b678e60bda17c1637ec90326906d76a.mp3*~data=user_id=0,application_id=42~hmac=352ecc56b96798ad619f86c0fb206d87bc3c6f115ef1a22a87ee0797c3c65810", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 13, ExternalTrackId = "908604652", Source = "Deezer", Title = "After Hours", ArtistId = 1, AlbumId = 1, DurationSeconds = 362, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/a/5/0/7a57adf1f07af04ff457e0ce2c509d4a.mp3?hdnea=exp=1776617519~acl=/api/1/1/7/a/5/0/7a57adf1f07af04ff457e0ce2c509d4a.mp3*~data=user_id=0,application_id=42~hmac=1e4509a0363289e980898f2e524c87380958bb8441f9086469edcf5213741652", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 14, ExternalTrackId = "908604662", Source = "Deezer", Title = "Until I Bleed Out", ArtistId = 1, AlbumId = 1, DurationSeconds = 190, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/a/2/0/fa2ebf166439e4ab97dc516c44c32645.mp3?hdnea=exp=1776617519~acl=/api/1/1/f/a/2/0/fa2ebf166439e4ab97dc516c44c32645.mp3*~data=user_id=0,application_id=42~hmac=be7c2e15e4ea6c37c842ce6baf6f7eaad6ec0b647fd1567467c0198590487b13", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 15, ExternalTrackId = "124603248", Source = "Deezer", Title = "Keep The Family Close", ArtistId = 2, AlbumId = 2, DurationSeconds = 331, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/0/3/0/e03276a402f16b01e822a51530e13820.mp3?hdnea=exp=1776617520~acl=/api/1/1/e/0/3/0/e03276a402f16b01e822a51530e13820.mp3*~data=user_id=0,application_id=42~hmac=8feff4f2400ff61bf567e0feb479afcdca9b99e505fd12e93d2a4d614627eaad", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 16, ExternalTrackId = "124603250", Source = "Deezer", Title = "9", ArtistId = 2, AlbumId = 2, DurationSeconds = 256, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/e/a/0/ceac2b0f817e0be28654b1cd6c6f6115.mp3?hdnea=exp=1776617520~acl=/api/1/1/c/e/a/0/ceac2b0f817e0be28654b1cd6c6f6115.mp3*~data=user_id=0,application_id=42~hmac=ea4441814ddafcc1c206d10dc36129ec33a18b7f233257d38a58bbdac0891246", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 17, ExternalTrackId = "124603252", Source = "Deezer", Title = "U With Me?", ArtistId = 2, AlbumId = 2, DurationSeconds = 297, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/9/d/0/69d7b40b640a219fb19741a7eed63ab8.mp3?hdnea=exp=1776617521~acl=/api/1/1/6/9/d/0/69d7b40b640a219fb19741a7eed63ab8.mp3*~data=user_id=0,application_id=42~hmac=787f3dce4a8b51f42a9d05d48cad3d0cc061f1cacd5936ad7126935fef65fc1d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 18, ExternalTrackId = "124603254", Source = "Deezer", Title = "Feel No Ways", ArtistId = 2, AlbumId = 2, DurationSeconds = 241, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/8/c/0/58cfd7592d8bb16d5de7a164e804390b.mp3?hdnea=exp=1776617522~acl=/api/1/1/5/8/c/0/58cfd7592d8bb16d5de7a164e804390b.mp3*~data=user_id=0,application_id=42~hmac=7ce538b70f2fc4078302bb86459757cd83278953169b5f4074dd7487ac6531c5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 19, ExternalTrackId = "124603256", Source = "Deezer", Title = "Hype", ArtistId = 2, AlbumId = 2, DurationSeconds = 209, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/6/4/0/d6493d02416696c3f00f5a00399047fc.mp3?hdnea=exp=1776617522~acl=/api/1/1/d/6/4/0/d6493d02416696c3f00f5a00399047fc.mp3*~data=user_id=0,application_id=42~hmac=b65e78d6c1c5c6e7a7ab28a79756ff312ca877d128a30eef379abb1861d40537", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 20, ExternalTrackId = "124603258", Source = "Deezer", Title = "Weston Road Flows", ArtistId = 2, AlbumId = 2, DurationSeconds = 253, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/5/b/0/35b7e20e329f3c9567714fba563daca4.mp3?hdnea=exp=1776617523~acl=/api/1/1/3/5/b/0/35b7e20e329f3c9567714fba563daca4.mp3*~data=user_id=0,application_id=42~hmac=48f6ef37062a98d17899af55ed888267b00777890c9960324d633d196cc828b5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 21, ExternalTrackId = "124603260", Source = "Deezer", Title = "Redemption", ArtistId = 2, AlbumId = 2, DurationSeconds = 334, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/f/a/0/2fa88b8bf7ffec483da07750905b0cf1.mp3?hdnea=exp=1776617524~acl=/api/1/1/2/f/a/0/2fa88b8bf7ffec483da07750905b0cf1.mp3*~data=user_id=0,application_id=42~hmac=6b5f9db49e4d372c8ac05e9e20d192122be31dab67b1936f0851bf610e2fe732", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 22, ExternalTrackId = "124603262", Source = "Deezer", Title = "With You", ArtistId = 2, AlbumId = 2, DurationSeconds = 195, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/4/4/0/c44cb2ba69b020feeb69c76345cc2db0.mp3?hdnea=exp=1776617524~acl=/api/1/1/c/4/4/0/c44cb2ba69b020feeb69c76345cc2db0.mp3*~data=user_id=0,application_id=42~hmac=0cab25e0ebdac343256e22372240b2409c315671c4a48bf27a4fbc19de2615ab", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 23, ExternalTrackId = "124603264", Source = "Deezer", Title = "Faithful", ArtistId = 2, AlbumId = 2, DurationSeconds = 290, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/e/a/0/7ea473e082e1b0255b3fa3c8f75179e7.mp3?hdnea=exp=1776617525~acl=/api/1/1/7/e/a/0/7ea473e082e1b0255b3fa3c8f75179e7.mp3*~data=user_id=0,application_id=42~hmac=6da8487a76754901c25fed4eb9c522173f95ea87c3f2daacfd94fe84e734f3c4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 24, ExternalTrackId = "124603266", Source = "Deezer", Title = "Still Here", ArtistId = 2, AlbumId = 2, DurationSeconds = 190, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/d/a/0/cda82207b80dca868d0fea714fa8fcfe.mp3?hdnea=exp=1776617526~acl=/api/1/1/c/d/a/0/cda82207b80dca868d0fea714fa8fcfe.mp3*~data=user_id=0,application_id=42~hmac=951c5a80db36186ee5d5a009b6ce0d490b21ca85f61cac07d652208c3a26ef54", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 25, ExternalTrackId = "124603268", Source = "Deezer", Title = "Controlla", ArtistId = 2, AlbumId = 2, DurationSeconds = 245, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/4/a/0/94a94baa6b342700c5d7a18b978bc351.mp3?hdnea=exp=1776617526~acl=/api/1/1/9/4/a/0/94a94baa6b342700c5d7a18b978bc351.mp3*~data=user_id=0,application_id=42~hmac=4a22ab53b0054c726041a0559b8c49918b2e195150c9b7cf26ef940f121b73d5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 26, ExternalTrackId = "124603270", Source = "Deezer", Title = "One Dance", ArtistId = 2, AlbumId = 2, DurationSeconds = 174, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/a/b/0/7ab0602a95834e7862f7ef2ee5389d88.mp3?hdnea=exp=1776617527~acl=/api/1/1/7/a/b/0/7ab0602a95834e7862f7ef2ee5389d88.mp3*~data=user_id=0,application_id=42~hmac=3905b678da337ecd6cbd4539f3eca74f987a1b061663a8ccc0649ee8952f60f8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 27, ExternalTrackId = "124603272", Source = "Deezer", Title = "Grammys", ArtistId = 2, AlbumId = 2, DurationSeconds = 220, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/b/6/0/8b62125bc18341eb974dbad67cf37bc5.mp3?hdnea=exp=1776617528~acl=/api/1/1/8/b/6/0/8b62125bc18341eb974dbad67cf37bc5.mp3*~data=user_id=0,application_id=42~hmac=dae9b56f0c4bd348cd704a555165419daf77b74c741ae08ce638ef08a65770b4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 28, ExternalTrackId = "124603274", Source = "Deezer", Title = "Childs Play", ArtistId = 2, AlbumId = 2, DurationSeconds = 241, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/e/5/0/7e5dd96f6a93cedfffd9650fdb999a70.mp3?hdnea=exp=1776617528~acl=/api/1/1/7/e/5/0/7e5dd96f6a93cedfffd9650fdb999a70.mp3*~data=user_id=0,application_id=42~hmac=c2bc0395a953067a0db0a82de5a75f6c5ecd43d87b952ed45e083cc01e9d5ac2", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 29, ExternalTrackId = "124603276", Source = "Deezer", Title = "Pop Style", ArtistId = 2, AlbumId = 2, DurationSeconds = 213, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/9/5/0/395f5a3d1bbd35b261b2771f73d0959f.mp3?hdnea=exp=1776617529~acl=/api/1/1/3/9/5/0/395f5a3d1bbd35b261b2771f73d0959f.mp3*~data=user_id=0,application_id=42~hmac=2aa18e74ae2dcb97accb98d08f423d14a04806ec9cb2e125108ce4032903bfaa", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 30, ExternalTrackId = "124603278", Source = "Deezer", Title = "Too Good", ArtistId = 2, AlbumId = 2, DurationSeconds = 263, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/1/5/0/f152d4afb75fae93e1f8702f2e6be4b3.mp3?hdnea=exp=1776617529~acl=/api/1/1/f/1/5/0/f152d4afb75fae93e1f8702f2e6be4b3.mp3*~data=user_id=0,application_id=42~hmac=601713dc74388594b4571425c47a8ed2e9296a8dc175bd1218717b9e9a2f3641", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 31, ExternalTrackId = "124603280", Source = "Deezer", Title = "Summers Over Interlude", ArtistId = 2, AlbumId = 2, DurationSeconds = 106, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/8/a/0/68a32a4c35646cba8c7144e703a98921.mp3?hdnea=exp=1776617530~acl=/api/1/1/6/8/a/0/68a32a4c35646cba8c7144e703a98921.mp3*~data=user_id=0,application_id=42~hmac=1a7cb16f240c3860d02934b652238b5833f04bbde446864abfe27bc42f66a6bc", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 32, ExternalTrackId = "124603282", Source = "Deezer", Title = "Fire & Desire", ArtistId = 2, AlbumId = 2, DurationSeconds = 238, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/a/0/0/aa02d739f7ae296479dbf4c345a0f5ee.mp3?hdnea=exp=1776617531~acl=/api/1/1/a/a/0/0/aa02d739f7ae296479dbf4c345a0f5ee.mp3*~data=user_id=0,application_id=42~hmac=ef5198a80c4b02b94b5eae45786c2756584ff3ad197d39f3a5cc07af5c68e876", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 33, ExternalTrackId = "124603284", Source = "Deezer", Title = "Views", ArtistId = 2, AlbumId = 2, DurationSeconds = 312, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/1/1/0/61134f3b6bdcdc87b14bb81bc1fc97b4.mp3?hdnea=exp=1776617531~acl=/api/1/1/6/1/1/0/61134f3b6bdcdc87b14bb81bc1fc97b4.mp3*~data=user_id=0,application_id=42~hmac=b8861d79f9e57a8a05acd0b43ea49b89f71e8cc26f6060e9ae2195efdb239729", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 34, ExternalTrackId = "124603286", Source = "Deezer", Title = "Hotline Bling", ArtistId = 2, AlbumId = 2, DurationSeconds = 267, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/c/1/0/ac1d5a7a9a829c0332a6de32a0f8a90b.mp3?hdnea=exp=1776617532~acl=/api/1/1/a/c/1/0/ac1d5a7a9a829c0332a6de32a0f8a90b.mp3*~data=user_id=0,application_id=42~hmac=95268ede0d7c756f71672c91906479d54c768838ac40a09cf18d8a0e71bea410", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 35, ExternalTrackId = "2386586015", Source = "Deezer", Title = "HYAENA", ArtistId = 3, AlbumId = 3, DurationSeconds = 222, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/9/b/0/a9b20759828ddcd17b5ea7b4f3b6d558.mp3?hdnea=exp=1776617532~acl=/api/1/1/a/9/b/0/a9b20759828ddcd17b5ea7b4f3b6d558.mp3*~data=user_id=0,application_id=42~hmac=9861f9f4e951ee5f13a44a13270832e90ef7fba7c7ab720a3f830864f7f5eb6d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 36, ExternalTrackId = "2386586025", Source = "Deezer", Title = "THANK GOD", ArtistId = 3, AlbumId = 3, DurationSeconds = 184, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/8/b/0/78b30c444866caecd2da14b751a77ddf.mp3?hdnea=exp=1776617533~acl=/api/1/1/7/8/b/0/78b30c444866caecd2da14b751a77ddf.mp3*~data=user_id=0,application_id=42~hmac=b80584d6418b786d717baaae83629074a9be29cd8eb6c647e0e745704102f852", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 37, ExternalTrackId = "2386586035", Source = "Deezer", Title = "MODERN JAM feat. Teezo Touchdown", ArtistId = 3, AlbumId = 3, DurationSeconds = 255, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/3/6/0/3364592cf2e9b5052dd08a93899db8f3.mp3?hdnea=exp=1776617533~acl=/api/1/1/3/3/6/0/3364592cf2e9b5052dd08a93899db8f3.mp3*~data=user_id=0,application_id=42~hmac=3119c68a87cdccbd6c1385e8c0668d6c375f7b7ae59aae5ef47579e87f4cd620", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 38, ExternalTrackId = "2386586045", Source = "Deezer", Title = "MY EYES", ArtistId = 3, AlbumId = 3, DurationSeconds = 251, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/8/8/0/188200dafe673bb689b5ae2805f2e7a7.mp3?hdnea=exp=1776617534~acl=/api/1/1/1/8/8/0/188200dafe673bb689b5ae2805f2e7a7.mp3*~data=user_id=0,application_id=42~hmac=2eff795bcc48508a00ab3c02906d929b4a725cdefc1b7331b75ca5e70bbc8334", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 39, ExternalTrackId = "2386586055", Source = "Deezer", Title = "GOD'S COUNTRY", ArtistId = 3, AlbumId = 3, DurationSeconds = 127, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/3/3/0/7332f25ee7317928087e57164bec1932.mp3?hdnea=exp=1776617535~acl=/api/1/1/7/3/3/0/7332f25ee7317928087e57164bec1932.mp3*~data=user_id=0,application_id=42~hmac=bd6692c92f8b50f55c3157df5c27651a378ab2a674124e7cfe97dd50ab04da95", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 40, ExternalTrackId = "2386586065", Source = "Deezer", Title = "SIRENS", ArtistId = 3, AlbumId = 3, DurationSeconds = 204, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/7/c/0/e7c8ea4259d82c5499a83b6cb0ec41ca.mp3?hdnea=exp=1776617535~acl=/api/1/1/e/7/c/0/e7c8ea4259d82c5499a83b6cb0ec41ca.mp3*~data=user_id=0,application_id=42~hmac=d5700f4f62632b8f7ac867cd8c452ec3dcf5eec80691fd82903a187ead495ec5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 41, ExternalTrackId = "2386586075", Source = "Deezer", Title = "MELTDOWN (feat. Drake)", ArtistId = 3, AlbumId = 3, DurationSeconds = 246, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/e/d/0/aed443f26468d074264d84aede6b8c0f.mp3?hdnea=exp=1776617536~acl=/api/1/1/a/e/d/0/aed443f26468d074264d84aede6b8c0f.mp3*~data=user_id=0,application_id=42~hmac=e80c20a8dc7b8238933d3f25db22c7249f839a2794cec645f45661e526e3471a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 42, ExternalTrackId = "2386586085", Source = "Deezer", Title = "FE!N (feat. Playboi Carti)", ArtistId = 3, AlbumId = 3, DurationSeconds = 191, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/2/7/0/02700631951a33db90a0024531e2378d.mp3?hdnea=exp=1776617536~acl=/api/1/1/0/2/7/0/02700631951a33db90a0024531e2378d.mp3*~data=user_id=0,application_id=42~hmac=6082c278cba36f3e31a89115f827a5651aa58e924610dc63d04df5aa078dc621", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 43, ExternalTrackId = "2386586095", Source = "Deezer", Title = "DELRESTO (ECHOES) (feat. Beyonc�)", ArtistId = 3, AlbumId = 3, DurationSeconds = 274, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/f/2/0/5f28e1e7b206ba51b76a1b8787e2f70a.mp3?hdnea=exp=1776617537~acl=/api/1/1/5/f/2/0/5f28e1e7b206ba51b76a1b8787e2f70a.mp3*~data=user_id=0,application_id=42~hmac=2152cbd5ac8470be0925155badd55b9435082d65b9b26d7d849225299c270819", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 44, ExternalTrackId = "2386586105", Source = "Deezer", Title = "I KNOW ?", ArtistId = 3, AlbumId = 3, DurationSeconds = 211, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/e/7/0/9e79ca53ba0e2b159e96cf606a39a8b8.mp3?hdnea=exp=1776617537~acl=/api/1/1/9/e/7/0/9e79ca53ba0e2b159e96cf606a39a8b8.mp3*~data=user_id=0,application_id=42~hmac=7b81b4bebec46766e39857b0642cc32532b9c6bb80bdb59264f1c306dc985da2", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 45, ExternalTrackId = "2386586115", Source = "Deezer", Title = "TOPIA TWINS (feat. Rob49 & 21 Savage)", ArtistId = 3, AlbumId = 3, DurationSeconds = 223, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/f/8/0/5f828096bb48daf4cdbae11d3a8a02df.mp3?hdnea=exp=1776617538~acl=/api/1/1/5/f/8/0/5f828096bb48daf4cdbae11d3a8a02df.mp3*~data=user_id=0,application_id=42~hmac=ea8703588578b44556d6e3c9ceecb659b091802bd3541775bb1db831059f71a3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 46, ExternalTrackId = "2386586125", Source = "Deezer", Title = "CIRCUS MAXIMUS (feat. The Weeknd)", ArtistId = 3, AlbumId = 3, DurationSeconds = 258, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/e/5/0/be5aed5be944c6f3d388b8b55fecf993.mp3?hdnea=exp=1776617539~acl=/api/1/1/b/e/5/0/be5aed5be944c6f3d388b8b55fecf993.mp3*~data=user_id=0,application_id=42~hmac=bd19635716ff45d02ee14dd0591fb5de56d593d76de4874cbe54108a9cb20f36", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 47, ExternalTrackId = "2386586135", Source = "Deezer", Title = "PARASAIL (feat. Young Thug)", ArtistId = 3, AlbumId = 3, DurationSeconds = 154, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/9/3/0/9932a66888a945f8244acea64b001101.mp3?hdnea=exp=1776617539~acl=/api/1/1/9/9/3/0/9932a66888a945f8244acea64b001101.mp3*~data=user_id=0,application_id=42~hmac=cb0d842deb51515024c244812b80d062f024727e542eea0d272f3433fb04410d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 48, ExternalTrackId = "2386586145", Source = "Deezer", Title = "SKITZO (feat. Young Thug)", ArtistId = 3, AlbumId = 3, DurationSeconds = 366, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/c/0/0/ac086eb2871488783199c1d91f33dbad.mp3?hdnea=exp=1776617540~acl=/api/1/1/a/c/0/0/ac086eb2871488783199c1d91f33dbad.mp3*~data=user_id=0,application_id=42~hmac=012061a33dfa12c726a980c75875157fc9ff10f0bf9bc54c291ea18b7308f8e5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 49, ExternalTrackId = "2386586155", Source = "Deezer", Title = "LOST FOREVER (feat. Westside Gunn)", ArtistId = 3, AlbumId = 3, DurationSeconds = 163, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/a/c/0/2ac8f26a88229d862a1d5f2c8ab6f296.mp3?hdnea=exp=1776617541~acl=/api/1/1/2/a/c/0/2ac8f26a88229d862a1d5f2c8ab6f296.mp3*~data=user_id=0,application_id=42~hmac=9b34eb1f432be5774340ff01cea0ec11517ec6ef9beeff830590eec6d5e6aaf1", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 50, ExternalTrackId = "2386586165", Source = "Deezer", Title = "LOOOVE (feat. Kid Cudi)", ArtistId = 3, AlbumId = 3, DurationSeconds = 226, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/2/9/0/3298fdc0e7e8ee2e1975dde3fa093400.mp3?hdnea=exp=1776617541~acl=/api/1/1/3/2/9/0/3298fdc0e7e8ee2e1975dde3fa093400.mp3*~data=user_id=0,application_id=42~hmac=6fd46e48336f6471f9f14d58c280ddbcb20d1d71c9579c9c2f0278f3a5dee697", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 51, ExternalTrackId = "2386586175", Source = "Deezer", Title = "K-POP (feat. Bad Bunny & The Weeknd)", ArtistId = 3, AlbumId = 3, DurationSeconds = 185, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/0/b/0/a0b7fe82a6babe20c8e09d44ee9af4da.mp3?hdnea=exp=1776617542~acl=/api/1/1/a/0/b/0/a0b7fe82a6babe20c8e09d44ee9af4da.mp3*~data=user_id=0,application_id=42~hmac=466cd534bb55e01778a53ccc2415939cf9d425ea6136bcb6dcd355f90ed920da", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 52, ExternalTrackId = "2386586185", Source = "Deezer", Title = "TELEKINESIS (feat. SZA & Future)", ArtistId = 3, AlbumId = 3, DurationSeconds = 353, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/9/d/0/59dc18cc9647c2878e0b5b453833a746.mp3?hdnea=exp=1776617542~acl=/api/1/1/5/9/d/0/59dc18cc9647c2878e0b5b453833a746.mp3*~data=user_id=0,application_id=42~hmac=ffb7baf5ffb044dd5e3d26e24b3d58f596dcb207dc4696f71f051e5593667b64", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 53, ExternalTrackId = "2386586195", Source = "Deezer", Title = "TIL FURTHER NOTICE (feat. James Blake & 21 Savage)", ArtistId = 3, AlbumId = 3, DurationSeconds = 314, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/a/1/0/ca11b0334cc356b4e0e9fe8feca8702b.mp3?hdnea=exp=1776617543~acl=/api/1/1/c/a/1/0/ca11b0334cc356b4e0e9fe8feca8702b.mp3*~data=user_id=0,application_id=42~hmac=405d7bef70803912adf3d2982b8d7966b6c2303563d7353787a67cb2c1c5d143", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 54, ExternalTrackId = "630827222", Source = "Deezer", Title = "Good Morning", ArtistId = 4, AlbumId = 4, DurationSeconds = 195, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/5/4/0/b54c0f0ffe49c85ba4e908dd1ea45dac.mp3?hdnea=exp=1776617544~acl=/api/1/1/b/5/4/0/b54c0f0ffe49c85ba4e908dd1ea45dac.mp3*~data=user_id=0,application_id=42~hmac=5757281b98889fd79e799149aba3ac500c815d47ad8a6a785ef98d7b3017ccce", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 55, ExternalTrackId = "630827232", Source = "Deezer", Title = "Champion", ArtistId = 4, AlbumId = 4, DurationSeconds = 167, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/8/c/0/88cf6edbde90f329dd5715ec3fe39b9d.mp3?hdnea=exp=1776617544~acl=/api/1/1/8/8/c/0/88cf6edbde90f329dd5715ec3fe39b9d.mp3*~data=user_id=0,application_id=42~hmac=561bad8510193cf9871213b148ea9b7e299e613250adf837813f3993ba9f23f8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 56, ExternalTrackId = "630827242", Source = "Deezer", Title = "Stronger", ArtistId = 4, AlbumId = 4, DurationSeconds = 312, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/6/1/0/361276290572e6564c4aa3e265449d93.mp3?hdnea=exp=1776617545~acl=/api/1/1/3/6/1/0/361276290572e6564c4aa3e265449d93.mp3*~data=user_id=0,application_id=42~hmac=79d4b97f276250508c8ab36c776f6749dde2507d16255135e5bd24a91dddf307", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 57, ExternalTrackId = "630827252", Source = "Deezer", Title = "I Wonder", ArtistId = 4, AlbumId = 4, DurationSeconds = 243, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/c/a/0/4ca3eeae8607e2e342ed1a7b2d5313d8.mp3?hdnea=exp=1776617545~acl=/api/1/1/4/c/a/0/4ca3eeae8607e2e342ed1a7b2d5313d8.mp3*~data=user_id=0,application_id=42~hmac=58897f82f69b84f89e4b139f1a6e30e868d92c4a5ccebd5bc94670e96f0a64aa", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 58, ExternalTrackId = "630827262", Source = "Deezer", Title = "Good Life", ArtistId = 4, AlbumId = 4, DurationSeconds = 207, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/f/7/0/0f7acdb2ea5d6d16b91cd6a8ba3b8d80.mp3?hdnea=exp=1776617546~acl=/api/1/1/0/f/7/0/0f7acdb2ea5d6d16b91cd6a8ba3b8d80.mp3*~data=user_id=0,application_id=42~hmac=d4996cfd8336f762ab113185b03ba39393ed2157a441c4d44321f548a7ac0172", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 59, ExternalTrackId = "630827272", Source = "Deezer", Title = "Can't Tell Me Nothing", ArtistId = 4, AlbumId = 4, DurationSeconds = 274, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/6/1/0/761ad4e6c747d8805df22c5d50f4ae5c.mp3?hdnea=exp=1776617546~acl=/api/1/1/7/6/1/0/761ad4e6c747d8805df22c5d50f4ae5c.mp3*~data=user_id=0,application_id=42~hmac=be053151e14f088ebd5bdd2ee43f172dd709fde1668948ec1ed96cbab8203bd1", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 60, ExternalTrackId = "630827282", Source = "Deezer", Title = "Barry Bonds", ArtistId = 4, AlbumId = 4, DurationSeconds = 204, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/4/9/0/f49cad0f247146281804fad233160a6a.mp3?hdnea=exp=1776617547~acl=/api/1/1/f/4/9/0/f49cad0f247146281804fad233160a6a.mp3*~data=user_id=0,application_id=42~hmac=792ff882d8f2bf3839a8f46cc39147329ad09f2991b2804d000e288c071390e4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 61, ExternalTrackId = "630827292", Source = "Deezer", Title = "Drunk and Hot Girls", ArtistId = 4, AlbumId = 4, DurationSeconds = 313, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/9/5/0/0956306bee6ec6bfbb73d05bd5655c90.mp3?hdnea=exp=1776617548~acl=/api/1/1/0/9/5/0/0956306bee6ec6bfbb73d05bd5655c90.mp3*~data=user_id=0,application_id=42~hmac=4999505b75434ac0e30b0cdc6af8f451e7eaf485fa66bdcfc8a25414de27f346", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 62, ExternalTrackId = "630827302", Source = "Deezer", Title = "Flashing Lights", ArtistId = 4, AlbumId = 4, DurationSeconds = 237, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/1/9/0/c197747c4e0d080521c00e8efd392be5.mp3?hdnea=exp=1776617548~acl=/api/1/1/c/1/9/0/c197747c4e0d080521c00e8efd392be5.mp3*~data=user_id=0,application_id=42~hmac=63368fdc452c0e5eabb1ec7f845479340dd071d61ef246fdad4284021c3f9937", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 63, ExternalTrackId = "630827312", Source = "Deezer", Title = "Everything I Am", ArtistId = 4, AlbumId = 4, DurationSeconds = 227, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/2/d/0/32dc386759de6f31755b8e1efae588d7.mp3?hdnea=exp=1776617549~acl=/api/1/1/3/2/d/0/32dc386759de6f31755b8e1efae588d7.mp3*~data=user_id=0,application_id=42~hmac=74fd5db37b756b52028748726040eb16987d020ccf9f58f17a1460f25c7b9d62", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 64, ExternalTrackId = "630827322", Source = "Deezer", Title = "The Glory", ArtistId = 4, AlbumId = 4, DurationSeconds = 212, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/f/b/0/1fb7d3b0356baa87b1deb7918e98e01b.mp3?hdnea=exp=1776617549~acl=/api/1/1/1/f/b/0/1fb7d3b0356baa87b1deb7918e98e01b.mp3*~data=user_id=0,application_id=42~hmac=6ff587d83a06d371d56ca7c0a10c1a7a88fefe189a4737838a0be977460d62dd", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 65, ExternalTrackId = "630827332", Source = "Deezer", Title = "Homecoming", ArtistId = 4, AlbumId = 4, DurationSeconds = 203, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/a/7/0/ea77f578dda311b63fe1207792be3a38.mp3?hdnea=exp=1776617550~acl=/api/1/1/e/a/7/0/ea77f578dda311b63fe1207792be3a38.mp3*~data=user_id=0,application_id=42~hmac=6daab1e5d86d3e1cb9e06c0e18dd35768b771804940f34945cff407b13ef3b52", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 66, ExternalTrackId = "630827342", Source = "Deezer", Title = "Big Brother", ArtistId = 4, AlbumId = 4, DurationSeconds = 287, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/4/b/0/44bf5fb7d174dcf3acea1801430911ba.mp3?hdnea=exp=1776617551~acl=/api/1/1/4/4/b/0/44bf5fb7d174dcf3acea1801430911ba.mp3*~data=user_id=0,application_id=42~hmac=ab8cbbb53b998fbb64f39fbd146614aaf30c1df9271bb5eb3eb7eb6fad744acc", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 67, ExternalTrackId = "630827352", Source = "Deezer", Title = "Good Night", ArtistId = 4, AlbumId = 4, DurationSeconds = 186, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/c/c/0/ccc7efbee3940a10dd0a3e32eef87390.mp3?hdnea=exp=1776617552~acl=/api/1/1/c/c/c/0/ccc7efbee3940a10dd0a3e32eef87390.mp3*~data=user_id=0,application_id=42~hmac=bb60b2a743274887561031d00d1e907be16e432ee3ca37008980eba1b6f8a762", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 68, ExternalTrackId = "14628993", Source = "Deezer", Title = "Foreword", ArtistId = 5, AlbumId = 5, DurationSeconds = 13, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/1/7/0/717f9db1d4cd21fbff9429ae7d5077f6.mp3?hdnea=exp=1776617552~acl=/api/1/1/7/1/7/0/717f9db1d4cd21fbff9429ae7d5077f6.mp3*~data=user_id=0,application_id=42~hmac=d4b55bfc10e6a4fec58383f69a6ad1e5be88e54739bbf9ecd1fffc43e40a6c35", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 69, ExternalTrackId = "14628994", Source = "Deezer", Title = "Don't Stay", ArtistId = 5, AlbumId = 5, DurationSeconds = 187, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/9/3/0/893158e9e533d955d94def003a4bb3d1.mp3?hdnea=exp=1776617553~acl=/api/1/1/8/9/3/0/893158e9e533d955d94def003a4bb3d1.mp3*~data=user_id=0,application_id=42~hmac=67aeb7135b07cbd60cab9d90c12f988824bc54181bc5dc82c7da97c1fb67b59e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 70, ExternalTrackId = "14628995", Source = "Deezer", Title = "Somewhere I Belong", ArtistId = 5, AlbumId = 5, DurationSeconds = 213, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/0/7/0/70748cd9030247f2f9063820bc1e5017.mp3?hdnea=exp=1776617553~acl=/api/1/1/7/0/7/0/70748cd9030247f2f9063820bc1e5017.mp3*~data=user_id=0,application_id=42~hmac=78f231996435bccb7ea6ad46e2fc2bf220e0768b3d03e0eb221ba6470ad61655", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 71, ExternalTrackId = "14628996", Source = "Deezer", Title = "Lying from You", ArtistId = 5, AlbumId = 5, DurationSeconds = 175, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/2/2/0/d2252ff4b9536eae31bfbdd862ccc9cd.mp3?hdnea=exp=1776617554~acl=/api/1/1/d/2/2/0/d2252ff4b9536eae31bfbdd862ccc9cd.mp3*~data=user_id=0,application_id=42~hmac=fcc4a9e13ebad93c76fefc1e90e040542be3b91e7d31837b3838554bee785e3b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 72, ExternalTrackId = "14628997", Source = "Deezer", Title = "Hit the Floor", ArtistId = 5, AlbumId = 5, DurationSeconds = 164, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/6/5/0/565f57d2dfcb73390022d89fcc9176a3.mp3?hdnea=exp=1776617555~acl=/api/1/1/5/6/5/0/565f57d2dfcb73390022d89fcc9176a3.mp3*~data=user_id=0,application_id=42~hmac=1428fbdeceb0ffbf3e0478288e47d43d1317154a4b926601550e21a96fd45574", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 73, ExternalTrackId = "14628998", Source = "Deezer", Title = "Easier to Run", ArtistId = 5, AlbumId = 5, DurationSeconds = 204, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/4/3/0/1431952d29234b66e546df29646c0ad1.mp3?hdnea=exp=1776617555~acl=/api/1/1/1/4/3/0/1431952d29234b66e546df29646c0ad1.mp3*~data=user_id=0,application_id=42~hmac=eb678a871557b2eb897c16942d178874eb8426e5cc0f2bf688dce9cf513f0c27", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 74, ExternalTrackId = "14628999", Source = "Deezer", Title = "Faint", ArtistId = 5, AlbumId = 5, DurationSeconds = 162, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/3/7/0/0377f1a5f8b1dd2dbd4aa7b3c0803f05.mp3?hdnea=exp=1776617556~acl=/api/1/1/0/3/7/0/0377f1a5f8b1dd2dbd4aa7b3c0803f05.mp3*~data=user_id=0,application_id=42~hmac=7483fa7c1f7a7d5706b7b421385770473b2032870222011655bd85456872f8d3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 75, ExternalTrackId = "14629000", Source = "Deezer", Title = "Figure.09", ArtistId = 5, AlbumId = 5, DurationSeconds = 197, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/9/9/0/6996061b8545e6fd977e59a258c2929f.mp3?hdnea=exp=1776617556~acl=/api/1/1/6/9/9/0/6996061b8545e6fd977e59a258c2929f.mp3*~data=user_id=0,application_id=42~hmac=3c8e470be09d61ef12e74d7a51f49d0edeea4bf9c3bedcd1656e39ee391e729f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 76, ExternalTrackId = "14629001", Source = "Deezer", Title = "Breaking the Habit", ArtistId = 5, AlbumId = 5, DurationSeconds = 196, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/4/6/0/046d901f9aaa58a09571d81770610d4c.mp3?hdnea=exp=1776617557~acl=/api/1/1/0/4/6/0/046d901f9aaa58a09571d81770610d4c.mp3*~data=user_id=0,application_id=42~hmac=dffc667d315e5d4c733b0e6c12ea740dd3396451e0de7659ff2000c49d3f588b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 77, ExternalTrackId = "14629002", Source = "Deezer", Title = "From the Inside", ArtistId = 5, AlbumId = 5, DurationSeconds = 175, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/7/d/0/47daed52ac560f7dd531cf9fca07112c.mp3?hdnea=exp=1776617558~acl=/api/1/1/4/7/d/0/47daed52ac560f7dd531cf9fca07112c.mp3*~data=user_id=0,application_id=42~hmac=5fb9b4c27f41662581c268aafa9b77fcb84f922a4feed01055e4959031e6e768", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 78, ExternalTrackId = "14629003", Source = "Deezer", Title = "Nobody's Listening", ArtistId = 5, AlbumId = 5, DurationSeconds = 178, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/6/c/0/76c801e139d29cb1ec955b9e6665e0d8.mp3?hdnea=exp=1776617558~acl=/api/1/1/7/6/c/0/76c801e139d29cb1ec955b9e6665e0d8.mp3*~data=user_id=0,application_id=42~hmac=a9ed2e973d573aa458f80a44bf0261a9279e0d28dc3fb1ce00ce924342b08cf3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 79, ExternalTrackId = "14629004", Source = "Deezer", Title = "Session", ArtistId = 5, AlbumId = 5, DurationSeconds = 144, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/c/4/0/3c4ec48f130c928f183880a1d2d51851.mp3?hdnea=exp=1776617559~acl=/api/1/1/3/c/4/0/3c4ec48f130c928f183880a1d2d51851.mp3*~data=user_id=0,application_id=42~hmac=93958db3c08d82698678a3816b9146004409a4fe9ea5bbcb599dcbae3c68806d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 80, ExternalTrackId = "14629005", Source = "Deezer", Title = "Numb", ArtistId = 5, AlbumId = 5, DurationSeconds = 187, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/2/b/0/42b05e42e6f684b6a2de860831cfa91a.mp3?hdnea=exp=1776617559~acl=/api/1/1/4/2/b/0/42b05e42e6f684b6a2de860831cfa91a.mp3*~data=user_id=0,application_id=42~hmac=081e99c2040f9ac27addc7b6a240648b1139a9ff5d833c69c34d1e869d0ea72d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 81, ExternalTrackId = "103052650", Source = "Deezer", Title = "Let It Happen", ArtistId = 6, AlbumId = 6, DurationSeconds = 469, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/a/e/0/6aee42a038480ecdcf2d6168f95810f2.mp3?hdnea=exp=1776617560~acl=/api/1/1/6/a/e/0/6aee42a038480ecdcf2d6168f95810f2.mp3*~data=user_id=0,application_id=42~hmac=77ceddc1a312826bb728eb3494373b91d680566a07377137f7edf1b3aa08e30a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 82, ExternalTrackId = "103052652", Source = "Deezer", Title = "Nangs", ArtistId = 6, AlbumId = 6, DurationSeconds = 106, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/8/3/0/083b5a8a6501d44ee896380ae6917a5c.mp3?hdnea=exp=1776617561~acl=/api/1/1/0/8/3/0/083b5a8a6501d44ee896380ae6917a5c.mp3*~data=user_id=0,application_id=42~hmac=4c744a3f4e59776e33edefcba493709f4f883a57b3600c4b5cbf98ec5683edda", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 83, ExternalTrackId = "103052654", Source = "Deezer", Title = "The Moment", ArtistId = 6, AlbumId = 6, DurationSeconds = 255, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/d/d/0/4dda1f0eff53e7852f7002245d1edfd5.mp3?hdnea=exp=1776617561~acl=/api/1/1/4/d/d/0/4dda1f0eff53e7852f7002245d1edfd5.mp3*~data=user_id=0,application_id=42~hmac=2cbca58401f182bea934cc283a39f046aa25ef0367f629d33c75fbbc94eba7f8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 84, ExternalTrackId = "103052656", Source = "Deezer", Title = "Yes I'm Changing", ArtistId = 6, AlbumId = 6, DurationSeconds = 270, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/1/d/0/61dbeb9b8059067d288580063c5b1dbe.mp3?hdnea=exp=1776617562~acl=/api/1/1/6/1/d/0/61dbeb9b8059067d288580063c5b1dbe.mp3*~data=user_id=0,application_id=42~hmac=2a1208f8502288e693b020db019c921709c7ee2f6477adbda32107e256dd0b78", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 85, ExternalTrackId = "103052658", Source = "Deezer", Title = "Eventually", ArtistId = 6, AlbumId = 6, DurationSeconds = 319, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/5/e/0/a5e04d13d8d31197426368c7d4856382.mp3?hdnea=exp=1776617562~acl=/api/1/1/a/5/e/0/a5e04d13d8d31197426368c7d4856382.mp3*~data=user_id=0,application_id=42~hmac=e85393da8872c7f6008ec24db761003a5588028a8da5cfc1af4f40e4d1caae9e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 86, ExternalTrackId = "103052660", Source = "Deezer", Title = "Gossip", ArtistId = 6, AlbumId = 6, DurationSeconds = 55, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/4/6/0/546fb9c87141be5295d0ad82c68f2768.mp3?hdnea=exp=1776617563~acl=/api/1/1/5/4/6/0/546fb9c87141be5295d0ad82c68f2768.mp3*~data=user_id=0,application_id=42~hmac=9970a958c8b06dcece3a88412d8743327ed52a52b80302350b1a15f9df3ea1d3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 87, ExternalTrackId = "103052662", Source = "Deezer", Title = "The Less I Know The Better", ArtistId = 6, AlbumId = 6, DurationSeconds = 217, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/7/e/0/d7e09f788f6834e38f61f8a589b0390a.mp3?hdnea=exp=1776617564~acl=/api/1/1/d/7/e/0/d7e09f788f6834e38f61f8a589b0390a.mp3*~data=user_id=0,application_id=42~hmac=afd6cb0207f2d5cc45171bd2f224da89b0d9bdc66002bf9e1a7ea55cb9bcc281", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 88, ExternalTrackId = "103052664", Source = "Deezer", Title = "Past Life", ArtistId = 6, AlbumId = 6, DurationSeconds = 227, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/3/e/0/03e860191b71fb57be47e228ae79accb.mp3?hdnea=exp=1776617564~acl=/api/1/1/0/3/e/0/03e860191b71fb57be47e228ae79accb.mp3*~data=user_id=0,application_id=42~hmac=370cc54b714d744c0e4320af838908d0bebb218a215ee94518f8fef617d3e290", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 89, ExternalTrackId = "103052666", Source = "Deezer", Title = "Disciples", ArtistId = 6, AlbumId = 6, DurationSeconds = 106, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/6/a/0/06ab2f12db7de63711a68c4717dadaa3.mp3?hdnea=exp=1776617565~acl=/api/1/1/0/6/a/0/06ab2f12db7de63711a68c4717dadaa3.mp3*~data=user_id=0,application_id=42~hmac=115ca174765e0e1e8557bfb9a6db94ee4b4e819cdb99c63ab79ec03743c796d2", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 90, ExternalTrackId = "103052668", Source = "Deezer", Title = "'Cause I'm A Man", ArtistId = 6, AlbumId = 6, DurationSeconds = 243, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/0/0/0/600888cdc730d73456a73e3820fa41db.mp3?hdnea=exp=1776617565~acl=/api/1/1/6/0/0/0/600888cdc730d73456a73e3820fa41db.mp3*~data=user_id=0,application_id=42~hmac=7088351f085cec288190b6e141399c425040135e43cf1ebfd8e8f3012e7550fb", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song
            {
                Id = 91,
                ExternalTrackId = "103052670",
                Source = "Deezer",
                Title = "Reality In Motion",
                ArtistId = 6,
                AlbumId = 6,
                DurationSeconds = 251,
                PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/2/4/0/c24e4b5dbbf209b0fce2b76f52bafd8c.mp3?hdnea=exp=1776617566~acl=/api/1/1/c/2/4/0/c24e4b5dbbf209b0fce2b76f52bafd8c.mp3*~data=user_id=0,application_id=42~hmac=7ae37722b7bb200722afd539b3f0fe2f1dac71464e6a4085d6dc78952732b577",
                CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8df49beb3e17ba2/250x250-000000-80-0-0.jpg",
                ReleaseDate = new DateTime(2015, 7, 17),
                IsActive = true,
                LastSyncedAt = new DateTime(2020, 3, 25),
                CreatedAt = new DateTime(2020, 3, 25)
            },

        new Song
        {
            Id = 92,
            ExternalTrackId = "103052672",
            Source = "Deezer",
            Title = "Love/Paranoia",
            ArtistId = 6,
            AlbumId = 6,
            DurationSeconds = 184,
            PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/a/5/0/6a52fe0122b5aa1525128feeb11680d2.mp3?hdnea=exp=1776617567~acl=/api/1/1/6/a/5/0/6a52fe0122b5aa1525128feeb11680d2.mp3*~data=user_id=0,application_id=42~hmac=c375445bfd9f3cda92af2db2d66608ee938045cde7908a623ec62ab1cfedc1b2",
            CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8df49beb3e17ba2/250x250-000000-80-0-0.jpg",
            ReleaseDate = new DateTime(2015, 7, 17),
            IsActive = true,
            LastSyncedAt = new DateTime(2020, 3, 25),
            CreatedAt = new DateTime(2020, 3, 25)
        },

            new Song { Id = 93, ExternalTrackId = "871688492", Source = "Deezer", Title = "State Of Grace", ArtistId = 7, AlbumId = 7, DurationSeconds = 296, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/1/4/0/e143dded018f40df1da7d7875dbd86fb.mp3?hdnea=exp=1776617567~acl=/api/1/1/e/1/4/0/e143dded018f40df1da7d7875dbd86fb.mp3*~data=user_id=0,application_id=42~hmac=8fca17524dcf461596da4a8a7c875603cdb79d9dede039021fdeb94887954ffe", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 94, ExternalTrackId = "871688502", Source = "Deezer", Title = "Red", ArtistId = 7, AlbumId = 7, DurationSeconds = 223, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/2/b/0/d2b535dee5924564ea4760e3306bf6e5.mp3?hdnea=exp=1776617568~acl=/api/1/1/d/2/b/0/d2b535dee5924564ea4760e3306bf6e5.mp3*~data=user_id=0,application_id=42~hmac=649a64a7b8889f11713dc0e906af45722c468f72cb64be7348113a661ee553b8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 95, ExternalTrackId = "871688512", Source = "Deezer", Title = "Treacherous", ArtistId = 7, AlbumId = 7, DurationSeconds = 243, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/5/c/0/55c7631b916a685ed7a3632ed57fc5a3.mp3?hdnea=exp=1776617568~acl=/api/1/1/5/5/c/0/55c7631b916a685ed7a3632ed57fc5a3.mp3*~data=user_id=0,application_id=42~hmac=45e1e1a48f2a2fe9e9f5fe095e92b6f08c0e3cfb41228a6c50a5f5642c2d0ac5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 96, ExternalTrackId = "871688522", Source = "Deezer", Title = "I Knew You Were Trouble.", ArtistId = 7, AlbumId = 7, DurationSeconds = 219, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/d/f/0/ddfccbf4d5fecb65600b8f0436b5fac3.mp3?hdnea=exp=1776617569~acl=/api/1/1/d/d/f/0/ddfccbf4d5fecb65600b8f0436b5fac3.mp3*~data=user_id=0,application_id=42~hmac=4f771c5201b477be136f3844fdba08bd049cfd53b60b563e62099c2b9f7184d4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 97, ExternalTrackId = "871688532", Source = "Deezer", Title = "All Too Well", ArtistId = 7, AlbumId = 7, DurationSeconds = 329, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/6/a/0/96ab463bd3cb4f1a7ecdfebd0d9ab3e8.mp3?hdnea=exp=1776617570~acl=/api/1/1/9/6/a/0/96ab463bd3cb4f1a7ecdfebd0d9ab3e8.mp3*~data=user_id=0,application_id=42~hmac=cbd103484ef8bed84af3c6ea04efb2707b286248adf63b61937c49bfecf1a1c0", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 98, ExternalTrackId = "871688552", Source = "Deezer", Title = "22", ArtistId = 7, AlbumId = 7, DurationSeconds = 232, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/b/b/0/1bb154d64c32e57afa167a97231dab7b.mp3?hdnea=exp=1776617570~acl=/api/1/1/1/b/b/0/1bb154d64c32e57afa167a97231dab7b.mp3*~data=user_id=0,application_id=42~hmac=efd546d512bfad34fc27d8c79c911defe7e57415e5eb5613201e9b8c5c3298b1", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 99, ExternalTrackId = "871688562", Source = "Deezer", Title = "I Almost Do", ArtistId = 7, AlbumId = 7, DurationSeconds = 245, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/1/b/0/61bd34a0ea3295a24b0ff5ef6282989b.mp3?hdnea=exp=1776617571~acl=/api/1/1/6/1/b/0/61bd34a0ea3295a24b0ff5ef6282989b.mp3*~data=user_id=0,application_id=42~hmac=a3d1a6fe61fe73739d4453c4bd4a0c2becbc14d0526232d87e34601a1de6b4be", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 100, ExternalTrackId = "871688572", Source = "Deezer", Title = "We Are Never Ever Getting Back Together", ArtistId = 7, AlbumId = 7, DurationSeconds = 192, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/6/1/0/e61d21d2d2af4cb332a956bdaa032e7e.mp3?hdnea=exp=1776617571~acl=/api/1/1/e/6/1/0/e61d21d2d2af4cb332a956bdaa032e7e.mp3*~data=user_id=0,application_id=42~hmac=59dd5c80bb2274543e53677cc521e6006f06dbb22906ce8190af2c3b7c24fa43", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 101, ExternalTrackId = "871688582", Source = "Deezer", Title = "Stay Stay Stay", ArtistId = 7, AlbumId = 7, DurationSeconds = 206, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/0/5/0/205f2d133de9b2bdbb28ccffbc6210bd.mp3?hdnea=exp=1776617572~acl=/api/1/1/2/0/5/0/205f2d133de9b2bdbb28ccffbc6210bd.mp3*~data=user_id=0,application_id=42~hmac=513049421e0f64d7d940b038f4ab9f57d75ea9a539d1995edc986aff14e9246d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 102, ExternalTrackId = "871688602", Source = "Deezer", Title = "The Last Time", ArtistId = 7, AlbumId = 7, DurationSeconds = 299, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/9/3/0/a93ff841d5fc3a76635480b1ab3bf838.mp3?hdnea=exp=1776617573~acl=/api/1/1/a/9/3/0/a93ff841d5fc3a76635480b1ab3bf838.mp3*~data=user_id=0,application_id=42~hmac=05f5d009db187f5b42f9e57149b26fc2f8df02c458b3cee13ef87a64144f636d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 103, ExternalTrackId = "871688612", Source = "Deezer", Title = "Holy Ground", ArtistId = 7, AlbumId = 7, DurationSeconds = 203, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/3/4/0/c34ec755cb2690395b2360d42ab29b7f.mp3?hdnea=exp=1776617574~acl=/api/1/1/c/3/4/0/c34ec755cb2690395b2360d42ab29b7f.mp3*~data=user_id=0,application_id=42~hmac=2cf23691f14dc4887ac0cd86d915429dd1f739a091ab9d6636c323defc6c83ea", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 104, ExternalTrackId = "871688622", Source = "Deezer", Title = "Sad Beautiful Tragic", ArtistId = 7, AlbumId = 7, DurationSeconds = 285, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/5/3/0/7533dc0cc0472368ca8b6fa112f7907d.mp3?hdnea=exp=1776617574~acl=/api/1/1/7/5/3/0/7533dc0cc0472368ca8b6fa112f7907d.mp3*~data=user_id=0,application_id=42~hmac=a970d71e004438632f6080b78d291bd50d3200f57b9fae94d3e3fb68f94c59ce", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 105, ExternalTrackId = "871688632", Source = "Deezer", Title = "The Lucky One", ArtistId = 7, AlbumId = 7, DurationSeconds = 240, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/2/d/0/c2d37554388588a61d88313ba31558bc.mp3?hdnea=exp=1776617575~acl=/api/1/1/c/2/d/0/c2d37554388588a61d88313ba31558bc.mp3*~data=user_id=0,application_id=42~hmac=b5f48e0e3f946e94ed7ef72b6cddda578277b7ab9dc92b761428b9fa518350d0", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 106, ExternalTrackId = "871688642", Source = "Deezer", Title = "Everything Has Changed", ArtistId = 7, AlbumId = 7, DurationSeconds = 245, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/2/e/0/62e4eb6b515e7c14088db0460be54540.mp3?hdnea=exp=1776617575~acl=/api/1/1/6/2/e/0/62e4eb6b515e7c14088db0460be54540.mp3*~data=user_id=0,application_id=42~hmac=e54014d5a46264a4f75dd5b1f5298bdd84cbd8a145bc58c96c494888b77f4936", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 107, ExternalTrackId = "871688652", Source = "Deezer", Title = "Starlight", ArtistId = 7, AlbumId = 7, DurationSeconds = 221, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/6/c/0/e6c5e662865eb8a291a2021f4649c90e.mp3?hdnea=exp=1776617576~acl=/api/1/1/e/6/c/0/e6c5e662865eb8a291a2021f4649c90e.mp3*~data=user_id=0,application_id=42~hmac=0ad0a9fca3d5e7d39e4b319e1f8b1881262db3a34d112cec68ff730793285ff0", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 108, ExternalTrackId = "871688662", Source = "Deezer", Title = "Begin Again", ArtistId = 7, AlbumId = 7, DurationSeconds = 238, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/4/f/0/54f3dcdbb21b5bbf3960ac5d1af15d5b.mp3?hdnea=exp=1776617577~acl=/api/1/1/5/4/f/0/54f3dcdbb21b5bbf3960ac5d1af15d5b.mp3*~data=user_id=0,application_id=42~hmac=0c0c0eaf33314ead3f4596e7b20c35d1b7b2de11586b4c7bbc4a81d09ee86bd2", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 109, ExternalTrackId = "68097787", Source = "Deezer", Title = "Hotel California (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 391, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/e/0/0/de0b52925103149e94a7123d0c0cb6c4.mp3?hdnea=exp=1776617577~acl=/api/1/1/d/e/0/0/de0b52925103149e94a7123d0c0cb6c4.mp3*~data=user_id=0,application_id=42~hmac=00f10c1c76ed51395ac2a063842d7e8be6abe02b14bf2166966b4bedf5f26d47", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 110, ExternalTrackId = "68097788", Source = "Deezer", Title = "New Kid in Town (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 304, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/c/b/0/9cbc1329a9ad1ebb6374b5d4a18e3c96.mp3?hdnea=exp=1776617578~acl=/api/1/1/9/c/b/0/9cbc1329a9ad1ebb6374b5d4a18e3c96.mp3*~data=user_id=0,application_id=42~hmac=edf19c698c47b895267abe982f1e779cb21a026d650e0c46126d794eaa22a1a3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 111, ExternalTrackId = "68097789", Source = "Deezer", Title = "Life in the Fast Lane (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 286, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/6/c/0/56c1283f56c64135a7ac25ae74c65dbf.mp3?hdnea=exp=1776617579~acl=/api/1/1/5/6/c/0/56c1283f56c64135a7ac25ae74c65dbf.mp3*~data=user_id=0,application_id=42~hmac=65534c58703025054a257602849f0b2a530161ae472c78411915d8bbebdb1eb8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 112, ExternalTrackId = "68097790", Source = "Deezer", Title = "Wasted Time (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 296, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/a/8/0/4a8bb975ac08486c9bc9a1d7e8d5deba.mp3?hdnea=exp=1776617579~acl=/api/1/1/4/a/8/0/4a8bb975ac08486c9bc9a1d7e8d5deba.mp3*~data=user_id=0,application_id=42~hmac=f047ac6c453f353d5ace9022ef77dec45257d3f763f1dbbaf2956f3ce71eab20", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 113, ExternalTrackId = "68097791", Source = "Deezer", Title = "Wasted Time (Reprise) (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 83, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/9/9/0/1994e2a76c3e3ee511fe3efb378c7afb.mp3?hdnea=exp=1776617580~acl=/api/1/1/1/9/9/0/1994e2a76c3e3ee511fe3efb378c7afb.mp3*~data=user_id=0,application_id=42~hmac=30385dcd204c70d39e141f20d0177c5f55e4ac4a3012b353f0bc5babc179dac6", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 114, ExternalTrackId = "68097792", Source = "Deezer", Title = "Victim of Love (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 250, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/6/5/0/f65837617bf64640f68d34fe59fd97ac.mp3?hdnea=exp=1776617580~acl=/api/1/1/f/6/5/0/f65837617bf64640f68d34fe59fd97ac.mp3*~data=user_id=0,application_id=42~hmac=b43aa98bb2f9681950de92ff13a9cd3be40fb9023cef3527171f6a04c4db51cb", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 115, ExternalTrackId = "68097793", Source = "Deezer", Title = "Pretty Maids All in a Row (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 239, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/d/a/0/edab780e537055e2a6b4df5d3e0e52af.mp3?hdnea=exp=1776617581~acl=/api/1/1/e/d/a/0/edab780e537055e2a6b4df5d3e0e52af.mp3*~data=user_id=0,application_id=42~hmac=846dad5bf082f9595e99d811930c96d64c9cb35dafb5c523bba79a5ab569dd72", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 116, ExternalTrackId = "68097794", Source = "Deezer", Title = "Try and Love Again (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 311, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/7/c/0/67c396a284944bdfa4e06259ae32fb06.mp3?hdnea=exp=1776617582~acl=/api/1/1/6/7/c/0/67c396a284944bdfa4e06259ae32fb06.mp3*~data=user_id=0,application_id=42~hmac=e88f46a5d9967d55524a27356f3e7b58845b4990bf972ef4026ff58abda0a40a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

        new Song
        {
            Id = 117,
            ExternalTrackId = "68097794",
            Source = "Deezer",
            Title = "The Last Resort (2013 Remaster)",
            ArtistId = 8,
            AlbumId = 8,
            DurationSeconds = 444,
            PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/7/c/0/67c396a284944bdfa4e06259ae32fb06.mp3?hdnea=exp=1776617582~acl=/api/1/1/6/7/c/0/67c396a284944bdfa4e06259ae32fb06.mp3*~data=user_id=0,application_id=42~hmac=e88f46a5d9967d55524a27356f3e7b58845b4990bf972ef4026ff58abda0a40a",
            CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg",
            ReleaseDate = new DateTime(2006, 4, 3),
            IsActive = true,
            LastSyncedAt = new DateTime(2020, 4, 4),
            CreatedAt = new DateTime(2020, 4, 3)
        },

            new Song { Id = 118, ExternalTrackId = "68097795", Source = "Deezer", Title = "The Last Resort (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 444, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/a/f/0/7af3b3cf978942108fd0a70cb46fbb76.mp3?hdnea=exp=1776617582~acl=/api/1/1/7/a/f/0/7af3b3cf978942108fd0a70cb46fbb76.mp3*~data=user_id=0,application_id=42~hmac=29f9c73ba29a4a2801ee457983146571ca6e8f2bef253c40f75af273315122df", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 119, ExternalTrackId = "4315309", Source = "Deezer", Title = "The View From The Afternoon", ArtistId = 9, AlbumId = 9, DurationSeconds = 222, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/2/e/0/d2e923a328a2949826ed3934b75589ff.mp3?hdnea=exp=1776617583~acl=/api/1/1/d/2/e/0/d2e923a328a2949826ed3934b75589ff.mp3*~data=user_id=0,application_id=42~hmac=e9f79b984c623ed2d9fa7682edce9b21ff64c8c4a24f21ba24cbcd088069a1d9", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 120, ExternalTrackId = "4315310", Source = "Deezer", Title = "I Bet You Look Good On The Dancefloor", ArtistId = 9, AlbumId = 9, DurationSeconds = 173, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/6/7/0/667973ef99272d4622145ddddd4f1201.mp3?hdnea=exp=1776617584~acl=/api/1/1/6/6/7/0/667973ef99272d4622145ddddd4f1201.mp3*~data=user_id=0,application_id=42~hmac=436bc879ebd2ada13c15286b96993b2c4b89899dfe85211de66d42376c29f02f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 121, ExternalTrackId = "4315311", Source = "Deezer", Title = "Fake Tales Of San Francisco", ArtistId = 9, AlbumId = 9, DurationSeconds = 177, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/d/1/0/9d1e9a0a40f51f6920a15bc6df6f6b4a.mp3?hdnea=exp=1776617584~acl=/api/1/1/9/d/1/0/9d1e9a0a40f51f6920a15bc6df6f6b4a.mp3*~data=user_id=0,application_id=42~hmac=ea3dc5a96a82dc19c621042b7d8badc0f947831ca758b825c8689ff0208f1491", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 122, ExternalTrackId = "4315312", Source = "Deezer", Title = "Dancing Shoes", ArtistId = 9, AlbumId = 9, DurationSeconds = 141, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/3/9/0/539215bf3c1d09e9c1e02742b2fd65bd.mp3?hdnea=exp=1776617585~acl=/api/1/1/5/3/9/0/539215bf3c1d09e9c1e02742b2fd65bd.mp3*~data=user_id=0,application_id=42~hmac=b2a48d455243bf4ee542c267873690492c36f286237569b865f2464078e0ae10", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 123, ExternalTrackId = "4315313", Source = "Deezer", Title = "You Probably Couldn't See For The Lights But You Were Staring Straight At Me", ArtistId = 9, AlbumId = 9, DurationSeconds = 130, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/f/a/0/7faf609e17d117ee64d6822f7f6b4b73.mp3?hdnea=exp=1776617585~acl=/api/1/1/7/f/a/0/7faf609e17d117ee64d6822f7f6b4b73.mp3*~data=user_id=0,application_id=42~hmac=1d3b4bd3ecd31ed7dee535539b613db4ba9c665dd9b6699ed01b0a3531b48a9a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 124, ExternalTrackId = "4315314", Source = "Deezer", Title = "Still Take You Home", ArtistId = 9, AlbumId = 9, DurationSeconds = 173, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/5/a/0/a5a974d3b2abfe6333bb489dab641e4e.mp3?hdnea=exp=1776617586~acl=/api/1/1/a/5/a/0/a5a974d3b2abfe6333bb489dab641e4e.mp3*~data=user_id=0,application_id=42~hmac=7576559038275466dfea64d1ec7a5bc7ecb8e2a61c8117a735ed7c40a4baa758", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 125, ExternalTrackId = "4315315", Source = "Deezer", Title = "Riot Van", ArtistId = 9, AlbumId = 9, DurationSeconds = 134, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/e/4/0/3e4bbe53e8f2bc74a4429d0588136580.mp3?hdnea=exp=1776617587~acl=/api/1/1/3/e/4/0/3e4bbe53e8f2bc74a4429d0588136580.mp3*~data=user_id=0,application_id=42~hmac=e901d605de6c06320478e0b38017237c844c48091e2297134c94e7d4c01952ab", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 126, ExternalTrackId = "4315316", Source = "Deezer", Title = "Red Light Indicates Doors Are Secured", ArtistId = 9, AlbumId = 9, DurationSeconds = 143, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/9/b/0/b9bd429cecbf66166d25acc2777faa60.mp3?hdnea=exp=1776617587~acl=/api/1/1/b/9/b/0/b9bd429cecbf66166d25acc2777faa60.mp3*~data=user_id=0,application_id=42~hmac=c2d30aa346ba51cb6b1282e97a1ccf62fbfe8272f5bc715735a92055bfc431ce", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 127, ExternalTrackId = "4315317", Source = "Deezer", Title = "Mardy Bum", ArtistId = 9, AlbumId = 9, DurationSeconds = 175, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/f/2/0/3f2e4b9651387376f1d9bd5c3511052f.mp3?hdnea=exp=1776617588~acl=/api/1/1/3/f/2/0/3f2e4b9651387376f1d9bd5c3511052f.mp3*~data=user_id=0,application_id=42~hmac=788c78a9865b7089b7971b564c2200b32c9f02b12ec959722a96cc30aa7c0b56", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 128, ExternalTrackId = "4315318", Source = "Deezer", Title = "Perhaps Vampires Is A Bit Strong But...", ArtistId = 9, AlbumId = 9, DurationSeconds = 268, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/7/e/0/57eace7efac96e4f368e65f82a0a5770.mp3?hdnea=exp=1776617589~acl=/api/1/1/5/7/e/0/57eace7efac96e4f368e65f82a0a5770.mp3*~data=user_id=0,application_id=42~hmac=a7da4d897082504db613a4eb3c59f911909b61aa01179aa2504de48f595c6ebd", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 129, ExternalTrackId = "4315319", Source = "Deezer", Title = "When The Sun Goes Down", ArtistId = 9, AlbumId = 9, DurationSeconds = 202, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/f/6/0/ff639e85a24b793f1fc05d6a037809b1.mp3?hdnea=exp=1776617589~acl=/api/1/1/f/f/6/0/ff639e85a24b793f1fc05d6a037809b1.mp3*~data=user_id=0,application_id=42~hmac=576a70e4cdde3d89aa634d94a165b7f65677fb6af2a4d7a41ebad2b07cacdcaf", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 130, ExternalTrackId = "4315320", Source = "Deezer", Title = "From The Ritz To The Rubble", ArtistId = 9, AlbumId = 9, DurationSeconds = 193, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/8/4/0/b841d08586960f786ed9c1ac56071e19.mp3?hdnea=exp=1776617590~acl=/api/1/1/b/8/4/0/b841d08586960f786ed9c1ac56071e19.mp3*~data=user_id=0,application_id=42~hmac=563d9df7ed03731c45ce572d2c436e04b09f6d7fc2dc08ae75aad26a5d8e5efd", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 131, ExternalTrackId = "4315321", Source = "Deezer", Title = "A Certain Romance", ArtistId = 9, AlbumId = 9, DurationSeconds = 331, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/6/0/0/2605aa8d5279dc6edc91af9a020dd877.mp3?hdnea=exp=1776617590~acl=/api/1/1/2/6/0/0/2605aa8d5279dc6edc91af9a020dd877.mp3*~data=user_id=0,application_id=42~hmac=519239e3167fa3eef79e6897c46fb4c8b4499cb0dcc879a24a220ba452a463bb", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 132, ExternalTrackId = "75526533", Source = "Deezer", Title = "Intro", ArtistId = 10, AlbumId = 10, DurationSeconds = 65, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/3/3/0/133cc25e5d911544c94dd2e9a538db27.mp3?hdnea=exp=1776617591~acl=/api/1/1/1/3/3/0/133cc25e5d911544c94dd2e9a538db27.mp3*~data=user_id=0,application_id=42~hmac=727e192b558c6e7ed0925da1eb2a2fa1a8f5118f3eaccf5b6fc9d1c2555d701a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 133, ExternalTrackId = "75526534", Source = "Deezer", Title = "When A Fire Starts To Burn", ArtistId = 10, AlbumId = 10, DurationSeconds = 284, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/6/e/0/46e06f386da4c60e6b04fee91f696ede.mp3?hdnea=exp=1776617592~acl=/api/1/1/4/6/e/0/46e06f386da4c60e6b04fee91f696ede.mp3*~data=user_id=0,application_id=42~hmac=10a7491985b0c48ee48d954c390239a8d6b6a5347c152de179f6cc06aafaa73a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 134, ExternalTrackId = "75526535", Source = "Deezer", Title = "Latch", ArtistId = 10, AlbumId = 10, DurationSeconds = 257, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/e/0/0/fe0f0222e7b333f0505f6d1b11707958.mp3?hdnea=exp=1776617592~acl=/api/1/1/f/e/0/0/fe0f0222e7b333f0505f6d1b11707958.mp3*~data=user_id=0,application_id=42~hmac=000ebb58c4bd03d4dd2168e3b1944ce64e9a29e18a21a3ff3c278a4cc0920623", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 135, ExternalTrackId = "75526536", Source = "Deezer", Title = "For You", ArtistId = 10, AlbumId = 10, DurationSeconds = 269, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/3/0/0/3303724374cc73952b03f6c7603a2517.mp3?hdnea=exp=1776617593~acl=/api/1/1/3/3/0/0/3303724374cc73952b03f6c7603a2517.mp3*~data=user_id=0,application_id=42~hmac=82ab6dc0efb7d150c0be96a627665d6299c22c809842ffaf590a095950680f97", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 136, ExternalTrackId = "75526537", Source = "Deezer", Title = "White Noise", ArtistId = 10, AlbumId = 10, DurationSeconds = 278, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/d/c/0/1dc625af3183c86072a5a22c8c1a7659.mp3?hdnea=exp=1776617594~acl=/api/1/1/1/d/c/0/1dc625af3183c86072a5a22c8c1a7659.mp3*~data=user_id=0,application_id=42~hmac=7f38771deacfd7c5976c59927c5b694a866bfe2ce6449745f28a6055a28a3cc4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 137, ExternalTrackId = "75526538", Source = "Deezer", Title = "Defeated No More", ArtistId = 10, AlbumId = 10, DurationSeconds = 368, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/0/6/0/2060f785b6d6ae413904bd2100bffdc4.mp3?hdnea=exp=1776617594~acl=/api/1/1/2/0/6/0/2060f785b6d6ae413904bd2100bffdc4.mp3*~data=user_id=0,application_id=42~hmac=bb3400989ca5f28cb6289e5cfc36841e7d98aced5ff6b41119c4d156113e1ea5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 138, ExternalTrackId = "75526539", Source = "Deezer", Title = "Stimulation", ArtistId = 10, AlbumId = 10, DurationSeconds = 320, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/9/8/0/e98af40519d709bd6eeec87c808c646f.mp3?hdnea=exp=1776617595~acl=/api/1/1/e/9/8/0/e98af40519d709bd6eeec87c808c646f.mp3*~data=user_id=0,application_id=42~hmac=e386f3d78e54e553e5ea41ac1d5f0e2a35468a818bcd88591f2acb6297c03ef8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 139, ExternalTrackId = "75526540", Source = "Deezer", Title = "Voices", ArtistId = 10, AlbumId = 10, DurationSeconds = 249, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/0/4/0/a04f32abe8d12145a679724c0bb86a00.mp3?hdnea=exp=1776617595~acl=/api/1/1/a/0/4/0/a04f32abe8d12145a679724c0bb86a00.mp3*~data=user_id=0,application_id=42~hmac=c0223832d15ed50c60bc873678d21498569a15597837ddf2e6ff35016feee33b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 140, ExternalTrackId = "75526541", Source = "Deezer", Title = "Second Chance", ArtistId = 10, AlbumId = 10, DurationSeconds = 151, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/c/d/0/5cd785ba5b3a8685d5d28aba8bdfd2c8.mp3?hdnea=exp=1776617596~acl=/api/1/1/5/c/d/0/5cd785ba5b3a8685d5d28aba8bdfd2c8.mp3*~data=user_id=0,application_id=42~hmac=e67d84280b783efd93c08a4c85cd5fe82eb53922898c229f3ed2d5b8f031eab3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 141, ExternalTrackId = "75526542", Source = "Deezer", Title = "Grab Her!", ArtistId = 10, AlbumId = 10, DurationSeconds = 313, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/e/f/0/9ef7927a1ecba34e0821270f9f286157.mp3?hdnea=exp=1776617597~acl=/api/1/1/9/e/f/0/9ef7927a1ecba34e0821270f9f286157.mp3*~data=user_id=0,application_id=42~hmac=1cce59b7253ffb124c3bdb9d9ea1ad26dd74c66af3307c1f6b1d5bee32e91914", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 142, ExternalTrackId = "75526543", Source = "Deezer", Title = "You & Me", ArtistId = 10, AlbumId = 10, DurationSeconds = 266, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/8/c/0/98c130a1544d30d58eb8b317b229df05.mp3?hdnea=exp=1776617597~acl=/api/1/1/9/8/c/0/98c130a1544d30d58eb8b317b229df05.mp3*~data=user_id=0,application_id=42~hmac=f79bdead624e19d3ae9c2a6b7cd60bf07785d07380ee0bb51d05adc48ecf2f30", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 143, ExternalTrackId = "75526544", Source = "Deezer", Title = "January", ArtistId = 10, AlbumId = 10, DurationSeconds = 355, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/f/8/0/2f88377863d02ad6eaf61c1b58f15946.mp3?hdnea=exp=1776617598~acl=/api/1/1/2/f/8/0/2f88377863d02ad6eaf61c1b58f15946.mp3*~data=user_id=0,application_id=42~hmac=d1f025b375e42ce6a56438df3ef3256e0a12207a822a2037407cd04597934582", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 144, ExternalTrackId = "75526545", Source = "Deezer", Title = "Confess To Me", ArtistId = 10, AlbumId = 10, DurationSeconds = 250, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/1/4/0/014c708b03625c2a623c7a027cf1ac1c.mp3?hdnea=exp=1776617599~acl=/api/1/1/0/1/4/0/014c708b03625c2a623c7a027cf1ac1c.mp3*~data=user_id=0,application_id=42~hmac=0dbbcbf3087f81978d3268f1539e4837fec01f42dcd51b57657f6cd37003c95d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 145, ExternalTrackId = "75526546", Source = "Deezer", Title = "Help Me Lose My Mind", ArtistId = 10, AlbumId = 10, DurationSeconds = 244, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/5/3/0/d53e5ab14ccba0d2344898022d763f97.mp3?hdnea=exp=1776617599~acl=/api/1/1/d/5/3/0/d53e5ab14ccba0d2344898022d763f97.mp3*~data=user_id=0,application_id=42~hmac=cb4f1212e26267960c6e823a71f39c866ea11cee1dc75c8d0ed70c9c6d6d12b3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 146, ExternalTrackId = "75526547", Source = "Deezer", Title = "Boiling", ArtistId = 10, AlbumId = 10, DurationSeconds = 227, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/5/4/0/75417ea5da5565b6fe870ec1df44444d.mp3?hdnea=exp=1776617600~acl=/api/1/1/7/5/4/0/75417ea5da5565b6fe870ec1df44444d.mp3*~data=user_id=0,application_id=42~hmac=c0662353e2595f96d6890cdb320537aeada8e742188076d93f627126ef6ebc25", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 147, ExternalTrackId = "75526548", Source = "Deezer", Title = "What's In Your Head", ArtistId = 10, AlbumId = 10, DurationSeconds = 330, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/9/4/0/c947c50fe02aab49f579249b105bad5e.mp3?hdnea=exp=1776617601~acl=/api/1/1/c/9/4/0/c947c50fe02aab49f579249b105bad5e.mp3*~data=user_id=0,application_id=42~hmac=ff505e570aba1c5f6de53904d5b540c5fc49ad0f46dd3b03cf10abcc9dcfe2d3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 148, ExternalTrackId = "75526549", Source = "Deezer", Title = "Tenderly", ArtistId = 10, AlbumId = 10, DurationSeconds = 304, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/e/9/0/fe93615cbd196e393ac3b39027d8d378.mp3?hdnea=exp=1776617601~acl=/api/1/1/f/e/9/0/fe93615cbd196e393ac3b39027d8d378.mp3*~data=user_id=0,application_id=42~hmac=6e0b12849a3a5063dc20b18690476d3af836588289c141d68efa41f54f176ef5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 149, ExternalTrackId = "75526550", Source = "Deezer", Title = "Running (Disclosure Remix)", ArtistId = 10, AlbumId = 10, DurationSeconds = 331, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/8/7/0/e87bc58558db36c604c355f7863f6b7d.mp3?hdnea=exp=1776617602~acl=/api/1/1/e/8/7/0/e87bc58558db36c604c355f7863f6b7d.mp3*~data=user_id=0,application_id=42~hmac=b3519e94e3a2ed8c5b79d328220103666a9cfce8e474209db383ac2f0319c60b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 150, ExternalTrackId = "75526551", Source = "Deezer", Title = "Apollo", ArtistId = 10, AlbumId = 10, DurationSeconds = 403, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/4/c/0/84c5324f5d3f997a201262768bb85b27.mp3?hdnea=exp=1776617603~acl=/api/1/1/8/4/c/0/84c5324f5d3f997a201262768bb85b27.mp3*~data=user_id=0,application_id=42~hmac=791ca96d9dc150cfe75f8ae632b89a49cd640d8c4138766a9db1fb7fad355c81", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 151, ExternalTrackId = "75526552", Source = "Deezer", Title = "Boiling (Dixon Rework)", ArtistId = 10, AlbumId = 10, DurationSeconds = 571, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/e/c/0/2ec4edd2e3b1a169787e2b595f812a4d.mp3?hdnea=exp=1776617604~acl=/api/1/1/2/e/c/0/2ec4edd2e3b1a169787e2b595f812a4d.mp3*~data=user_id=0,application_id=42~hmac=32281800975d50e7f85313d42979071eb9ab48a27cae60e088f3ffa201bc1f45", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 152, ExternalTrackId = "75526553", Source = "Deezer", Title = "Boiling (Medlar Remix)", ArtistId = 10, AlbumId = 10, DurationSeconds = 352, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/5/0/0/950e68336f913c8b6f9aa8783b39ec3f.mp3?hdnea=exp=1776617604~acl=/api/1/1/9/5/0/0/950e68336f913c8b6f9aa8783b39ec3f.mp3*~data=user_id=0,application_id=42~hmac=ee92c7050be35d6d67816d9433ce884c5db07e6356d7a6cadef68bc93ae107c7", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 153, ExternalTrackId = "75526554", Source = "Deezer", Title = "Control (Joe Goddard Remix)", ArtistId = 10, AlbumId = 10, DurationSeconds = 238, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/8/9/0/289b9c8ecb1246787f83ef8fb87a3122.mp3?hdnea=exp=1776617605~acl=/api/1/1/2/8/9/0/289b9c8ecb1246787f83ef8fb87a3122.mp3*~data=user_id=0,application_id=42~hmac=d6b82a18aefdd1511b4e8e5c3df2975b2edf27705ca5fc6f30e8c6a1679d13c2", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 154, ExternalTrackId = "75526555", Source = "Deezer", Title = "F For You (TEED Remix)", ArtistId = 10, AlbumId = 10, DurationSeconds = 355, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/4/6/0/c460b36a59f4b7e0bb516c03579fdabc.mp3?hdnea=exp=1776617605~acl=/api/1/1/c/4/6/0/c460b36a59f4b7e0bb516c03579fdabc.mp3*~data=user_id=0,application_id=42~hmac=7b138088f41d3aa9f4ec57e80e3c014ebef9aa3af3321dd31d01a7a98c04ef07", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 155, ExternalTrackId = "75526556", Source = "Deezer", Title = "Help Me Lose My Mind (Extended)", ArtistId = 10, AlbumId = 10, DurationSeconds = 428, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/8/f/0/38f069de5ca122c4eccfd7c5076f73f3.mp3?hdnea=exp=1776617606~acl=/api/1/1/3/8/f/0/38f069de5ca122c4eccfd7c5076f73f3.mp3*~data=user_id=0,application_id=42~hmac=3fb4c4c7581dbe974d5327bafc3b815d67d9358f11c6a20608b80b25b9f24cda", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 156, ExternalTrackId = "75526557", Source = "Deezer", Title = "Help Me Lose My Mind (Live)", ArtistId = 10, AlbumId = 10, DurationSeconds = 518, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/3/9/0/639b49d138b679b5d277812f57df12f7.mp3?hdnea=exp=1776617607~acl=/api/1/1/6/3/9/0/639b49d138b679b5d277812f57df12f7.mp3*~data=user_id=0,application_id=42~hmac=2d60a2c5fc10b66186392565ccb4e4374a2458ae732b52c32a5a8b8b3471be7e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 157, ExternalTrackId = "831216", Source = "Deezer", Title = "Wanna Be Startin' Somethin'", ArtistId = 11, AlbumId = 11, DurationSeconds = 363, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/8/3/0/083d0d153a1005e529732c857e87c2b5.mp3?hdnea=exp=1776617607~acl=/api/1/1/0/8/3/0/083d0d153a1005e529732c857e87c2b5.mp3*~data=user_id=0,application_id=42~hmac=bd3f5c443ba69888b5de42addd39bcd1b6652d75a93cbd233e98ce0a749dec00", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 158, ExternalTrackId = "831289", Source = "Deezer", Title = "Baby Be Mine", ArtistId = 11, AlbumId = 11, DurationSeconds = 260, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/0/4/0/3048781b11102865fa4c55b273e93a80.mp3?hdnea=exp=1776617608~acl=/api/1/1/3/0/4/0/3048781b11102865fa4c55b273e93a80.mp3*~data=user_id=0,application_id=42~hmac=fde5b59d286d43c71c6c1698acd23342cc6818f7f3a7617d9f2744fe28f14eaf", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 159, ExternalTrackId = "831298", Source = "Deezer", Title = "The Girl Is Mine (with Paul McCartney)", ArtistId = 11, AlbumId = 11, DurationSeconds = 222, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/5/0/0/4502ef8b9064c79a6237009adcbbcae8.mp3?hdnea=exp=1776617608~acl=/api/1/1/4/5/0/0/4502ef8b9064c79a6237009adcbbcae8.mp3*~data=user_id=0,application_id=42~hmac=acbb688253aed0efa5c5b2fe815d348fac7ea937e6531a7f1740c52badd0ad54", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 160, ExternalTrackId = "831319", Source = "Deezer", Title = "Thriller", ArtistId = 11, AlbumId = 11, DurationSeconds = 358, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/9/0/0/890b3041e2531f03adc503d6adca4ef9.mp3?hdnea=exp=1776617609~acl=/api/1/1/8/9/0/0/890b3041e2531f03adc503d6adca4ef9.mp3*~data=user_id=0,application_id=42~hmac=81bd00c836f41117f77b748806f8a445669972fc5e3c34f0666e99abf7dd65c4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 161, ExternalTrackId = "831272", Source = "Deezer", Title = "Beat It", ArtistId = 11, AlbumId = 11, DurationSeconds = 258, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/4/1/0/541401d59e9f2670cc2542c23f38e106.mp3?hdnea=exp=1776617610~acl=/api/1/1/5/4/1/0/541401d59e9f2670cc2542c23f38e106.mp3*~data=user_id=0,application_id=42~hmac=a127c1d7c5b2239f216ff73e3f3af955914209b9039eacb7ba369902aea0207d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 162, ExternalTrackId = "831313", Source = "Deezer", Title = "Billie Jean", ArtistId = 11, AlbumId = 11, DurationSeconds = 293, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/2/1/0/221b7e29f881436680582c67c8fbbd75.mp3?hdnea=exp=1776617610~acl=/api/1/1/2/2/1/0/221b7e29f881436680582c67c8fbbd75.mp3*~data=user_id=0,application_id=42~hmac=2f83fa57e10417639ae2436194dfbdc4b5f54ecffe5733b18c6f1c83af967cf2", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 163, ExternalTrackId = "831196", Source = "Deezer", Title = "Human Nature", ArtistId = 11, AlbumId = 11, DurationSeconds = 245, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/c/3/0/fc3a3f0ec77b0f16d92589e508a0e135.mp3?hdnea=exp=1776617611~acl=/api/1/1/f/c/3/0/fc3a3f0ec77b0f16d92589e508a0e135.mp3*~data=user_id=0,application_id=42~hmac=2c377503c6d6720d68e9f545630158f3822fc282c22f01d90eb87e46f4f1df3a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 164, ExternalTrackId = "831206", Source = "Deezer", Title = "P.Y.T. (Pretty Young Thing)", ArtistId = 11, AlbumId = 11, DurationSeconds = 239, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/8/0/0/08054bb3e947e0852f55e8c4be5e0af6.mp3?hdnea=exp=1776617611~acl=/api/1/1/0/8/0/0/08054bb3e947e0852f55e8c4be5e0af6.mp3*~data=user_id=0,application_id=42~hmac=2a9d69cd8f8e9b3eb3e427a508cb2aab5cf3bab244bbd65ea5f5a48e11e5dd37", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 165, ExternalTrackId = "831190", Source = "Deezer", Title = "The Lady In My Life", ArtistId = 11, AlbumId = 11, DurationSeconds = 297, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/a/2/0/0a2cf4a4304f6a145f4947b904bce821.mp3?hdnea=exp=1776617612~acl=/api/1/1/0/a/2/0/0a2cf4a4304f6a145f4947b904bce821.mp3*~data=user_id=0,application_id=42~hmac=a3599cf9371bbc334143ec16620702d8470750a609999276d51a5cdfee5b1950", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 166, ExternalTrackId = "59509421", Source = "Deezer", Title = "Bad (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 247, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/c/c/0/9cc39411d9d91ac35fdc2a4263f1f2c3.mp3?hdnea=exp=1776617613~acl=/api/1/1/9/c/c/0/9cc39411d9d91ac35fdc2a4263f1f2c3.mp3*~data=user_id=0,application_id=42~hmac=8a29bb54e500fdc5d5c8d2e9256239902423756ed4ea933640556c9f3f02a264", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 167, ExternalTrackId = "59509431", Source = "Deezer", Title = "The Way You Make Me Feel (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 298, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/a/3/0/ba307e0431c20da997a7fe52cfec6b9a.mp3?hdnea=exp=1776617613~acl=/api/1/1/b/a/3/0/ba307e0431c20da997a7fe52cfec6b9a.mp3*~data=user_id=0,application_id=42~hmac=29b0e6f863ed9118f6be7d5e5372ea611a5f474286e14da15ccaa95439a4718a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 168, ExternalTrackId = "59509441", Source = "Deezer", Title = "Speed Demon (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 242, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/4/3/0/9436a20a25c9079cc2d14c877b07373d.mp3?hdnea=exp=1776617614~acl=/api/1/1/9/4/3/0/9436a20a25c9079cc2d14c877b07373d.mp3*~data=user_id=0,application_id=42~hmac=50d399414dbfab2a44a873322691927ede87023d8b829044b16756bef7c8cc2f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 169, ExternalTrackId = "59509451", Source = "Deezer", Title = "Liberian Girl (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 232, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/5/b/0/05b1bda38cea9e40b5d0e4624ce1b2ef.mp3?hdnea=exp=1776617614~acl=/api/1/1/0/5/b/0/05b1bda38cea9e40b5d0e4624ce1b2ef.mp3*~data=user_id=0,application_id=42~hmac=49f74cadb6bfc26475ae8825739cb3f74b32b69b7923c3672c696d6feb6b4eea", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 170, ExternalTrackId = "59509461", Source = "Deezer", Title = "Just Good Friends (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 246, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/7/6/0/a76806059ce7009b2a01256908b8e2e2.mp3?hdnea=exp=1776617615~acl=/api/1/1/a/7/6/0/a76806059ce7009b2a01256908b8e2e2.mp3*~data=user_id=0,application_id=42~hmac=4cd158d132e4427b6330878b73da1718abf24ab2f9557138bc3f5a0172910a55", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 171, ExternalTrackId = "59509471", Source = "Deezer", Title = "Another Part of Me (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 234, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/3/3/0/433756022d4553e1ff34a9528420dd1f.mp3?hdnea=exp=1776617616~acl=/api/1/1/4/3/3/0/433756022d4553e1ff34a9528420dd1f.mp3*~data=user_id=0,application_id=42~hmac=2596cea51fb2a62111a18bd8c85d56984bfbc10f7aec020b0f1a5baad633be5f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 172, ExternalTrackId = "59509481", Source = "Deezer", Title = "Man in the Mirror (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 318, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/d/7/0/2d7fb17b138a9939d42a7bdbec8e8669.mp3?hdnea=exp=1776617616~acl=/api/1/1/2/d/7/0/2d7fb17b138a9939d42a7bdbec8e8669.mp3*~data=user_id=0,application_id=42~hmac=dd2eb8dd3911e090979ed557215d6297f0985f01214438d58fed617f31a670b9", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 173, ExternalTrackId = "59509491", Source = "Deezer", Title = "I Just Can't Stop Loving You (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 251, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/5/c/0/b5ce218fb3a3d4b05ffaa987ef0844f4.mp3?hdnea=exp=1776617617~acl=/api/1/1/b/5/c/0/b5ce218fb3a3d4b05ffaa987ef0844f4.mp3*~data=user_id=0,application_id=42~hmac=f91a165d11a2ab458c393d372f6e096ba128dadfc518c19aadf8f31c6f3edde8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 174, ExternalTrackId = "59509501", Source = "Deezer", Title = "Dirty Diana (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 280, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/7/f/0/f7f963f9541ca90be3c6df52d84a2d49.mp3?hdnea=exp=1776617617~acl=/api/1/1/f/7/f/0/f7f963f9541ca90be3c6df52d84a2d49.mp3*~data=user_id=0,application_id=42~hmac=40a76286d1db24e745796b6025f5fea1eb356b3d5a6b2dad070e54f02b572cde", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 175, ExternalTrackId = "59509511", Source = "Deezer", Title = "Smooth Criminal (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 257, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/7/a/0/c7ad17b555cbd5bdab04dd474049910a.mp3?hdnea=exp=1776617618~acl=/api/1/1/c/7/a/0/c7ad17b555cbd5bdab04dd474049910a.mp3*~data=user_id=0,application_id=42~hmac=3d3d85025e2b6fdd2827191deb0c67360e6f684f0e39356db02ab78eb60cb9a4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 176, ExternalTrackId = "59509521", Source = "Deezer", Title = "Leave Me Alone (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 280, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/9/8/0/798c05e5cb0ddd385a4d98eb613692d7.mp3?hdnea=exp=1776617619~acl=/api/1/1/7/9/8/0/798c05e5cb0ddd385a4d98eb613692d7.mp3*~data=user_id=0,application_id=42~hmac=543232a98a412fcee97776db51d0b2b393b06b81e800fbb6e6ed813922e084bd", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 177, ExternalTrackId = "8086126", Source = "Deezer", Title = "Rolling in the Deep", ArtistId = 12, AlbumId = 13, DurationSeconds = 228, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/8/7/0/d87d2d1789c1b4d96992d8079a29360b.mp3?hdnea=exp=1776617619~acl=/api/1/1/d/8/7/0/d87d2d1789c1b4d96992d8079a29360b.mp3*~data=user_id=0,application_id=42~hmac=01bc67329bdb4b2428818581732fe43066abf1836c7e787933b54198c15983a5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 178, ExternalTrackId = "8086127", Source = "Deezer", Title = "Rumour Has It", ArtistId = 12, AlbumId = 13, DurationSeconds = 223, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/4/d/0/b4d2f187fac83bf248cdbab59d87e2a7.mp3?hdnea=exp=1776617620~acl=/api/1/1/b/4/d/0/b4d2f187fac83bf248cdbab59d87e2a7.mp3*~data=user_id=0,application_id=42~hmac=bcd4863b41e85e94873bb0a43978960205401415b1b7dcefea110b8203f7739a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 179, ExternalTrackId = "8086128", Source = "Deezer", Title = "Turning Tables", ArtistId = 12, AlbumId = 13, DurationSeconds = 250, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/4/0/0/8405eeaa5361b80e96c0c5892342e582.mp3?hdnea=exp=1776617621~acl=/api/1/1/8/4/0/0/8405eeaa5361b80e96c0c5892342e582.mp3*~data=user_id=0,application_id=42~hmac=6c68f5c04be4a23417af8647047d160c0154eee7b28089f12fffa797e45981c3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 180, ExternalTrackId = "8086129", Source = "Deezer", Title = "Don't You Remember", ArtistId = 12, AlbumId = 13, DurationSeconds = 243, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/3/1/0/0314e8777c7d48cce3cfee97cb9b6a8b.mp3?hdnea=exp=1776617621~acl=/api/1/1/0/3/1/0/0314e8777c7d48cce3cfee97cb9b6a8b.mp3*~data=user_id=0,application_id=42~hmac=5ffc8d6a782e0279afb1865917c746ed027b225a4b2357df0edaa28d3adf3860", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 181, ExternalTrackId = "8086130", Source = "Deezer", Title = "Set Fire to the Rain", ArtistId = 12, AlbumId = 13, DurationSeconds = 242, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/9/d/0/89d33940ac8fd32e481a6f7ac7fa891a.mp3?hdnea=exp=1776617622~acl=/api/1/1/8/9/d/0/89d33940ac8fd32e481a6f7ac7fa891a.mp3*~data=user_id=0,application_id=42~hmac=228e07c59522afed54c11b035bd4240c5fa9e6b3db1b258cb2c6e39ec4b85f4e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 182, ExternalTrackId = "8086131", Source = "Deezer", Title = "He Won't Go", ArtistId = 12, AlbumId = 13, DurationSeconds = 278, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/b/7/0/0b7189926f572a025891120147fee4af.mp3?hdnea=exp=1776617622~acl=/api/1/1/0/b/7/0/0b7189926f572a025891120147fee4af.mp3*~data=user_id=0,application_id=42~hmac=62620d96a2fb5d8a2c40d792a02fea40d657e5183fee30211c7ab4fb730e3161", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 183, ExternalTrackId = "8086132", Source = "Deezer", Title = "Take It All", ArtistId = 12, AlbumId = 13, DurationSeconds = 228, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/0/8/0/608a0596d4b59b4d61c859bd97f1be65.mp3?hdnea=exp=1776617623~acl=/api/1/1/6/0/8/0/608a0596d4b59b4d61c859bd97f1be65.mp3*~data=user_id=0,application_id=42~hmac=49f6d79d8fcc7326e396e476adbe706fe76149cfcb592494d75ded82b5a7c5de", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 184, ExternalTrackId = "8086133", Source = "Deezer", Title = "I'll Be Waiting", ArtistId = 12, AlbumId = 13, DurationSeconds = 241, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/e/1/0/fe172e21920e194d26dd2a5b2b407794.mp3?hdnea=exp=1776617624~acl=/api/1/1/f/e/1/0/fe172e21920e194d26dd2a5b2b407794.mp3*~data=user_id=0,application_id=42~hmac=04f6f2f9855fce14f5fa4c0adad8d5d927e4b4a2089de123b955b7fe5fcd96fb", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 185, ExternalTrackId = "8086134", Source = "Deezer", Title = "One and Only", ArtistId = 12, AlbumId = 13, DurationSeconds = 348, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/b/1/0/fb185649172bce60bd735ac66934e926.mp3?hdnea=exp=1776617624~acl=/api/1/1/f/b/1/0/fb185649172bce60bd735ac66934e926.mp3*~data=user_id=0,application_id=42~hmac=b6e95be45652d94a08f30ff618cfe11cfab472581d694c30c1e3c1e58020de4d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 186, ExternalTrackId = "8086135", Source = "Deezer", Title = "Lovesong", ArtistId = 12, AlbumId = 13, DurationSeconds = 316, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/9/a/0/59a13a394852f873470c7c76c435b208.mp3?hdnea=exp=1776617625~acl=/api/1/1/5/9/a/0/59a13a394852f873470c7c76c435b208.mp3*~data=user_id=0,application_id=42~hmac=23ffe2627f6c222732787c282d5a6d21e857ccd444de2f150238e07c8c1dcb05", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 187, ExternalTrackId = "8086136", Source = "Deezer", Title = "Someone Like You", ArtistId = 12, AlbumId = 13, DurationSeconds = 285, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/f/6/0/cf61cfa01f653ad6b8696a81c6e4d747.mp3?hdnea=exp=1776617625~acl=/api/1/1/c/f/6/0/cf61cfa01f653ad6b8696a81c6e4d747.mp3*~data=user_id=0,application_id=42~hmac=ad503567edd83646f0ab0b2a4a11a8bf50263abe9a99adb046865cda6a02d42a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 188, ExternalTrackId = "629899752", Source = "Deezer", Title = "imagine", ArtistId = 13, AlbumId = 14, DurationSeconds = 212, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/3/d/0/73ddd21c0022c283a28427d3e5645773.mp3?hdnea=exp=1776617626~acl=/api/1/1/7/3/d/0/73ddd21c0022c283a28427d3e5645773.mp3*~data=user_id=0,application_id=42~hmac=bc805d9851642d3f1f3d550ed9ff75858d836e0deab123c4573216be89902988", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 189, ExternalTrackId = "629899762", Source = "Deezer", Title = "needy", ArtistId = 13, AlbumId = 14, DurationSeconds = 171, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/5/e/0/f5e5a2594671b0e74df65fc6a508c6b2.mp3?hdnea=exp=1776617627~acl=/api/1/1/f/5/e/0/f5e5a2594671b0e74df65fc6a508c6b2.mp3*~data=user_id=0,application_id=42~hmac=50a78e9d067fa8ac7b29ab59f508c4857f2c5ec98a589299c25a162f7be27b72", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 190, ExternalTrackId = "629899772", Source = "Deezer", Title = "NASA", ArtistId = 13, AlbumId = 14, DurationSeconds = 182, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/d/1/0/cd1c37e0e53a77f9079eb7a3f2cf8d22.mp3?hdnea=exp=1776617627~acl=/api/1/1/c/d/1/0/cd1c37e0e53a77f9079eb7a3f2cf8d22.mp3*~data=user_id=0,application_id=42~hmac=f7addcaca2bfa90d8ea228f2789130fb3c4bd5320a13e1b224aa65c27234ef93", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 191, ExternalTrackId = "629899782", Source = "Deezer", Title = "bloodline", ArtistId = 13, AlbumId = 14, DurationSeconds = 215, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/c/d/0/7cdfda0ecb26a93f5341e208b53eb196.mp3?hdnea=exp=1776617628~acl=/api/1/1/7/c/d/0/7cdfda0ecb26a93f5341e208b53eb196.mp3*~data=user_id=0,application_id=42~hmac=14bff926b9f057977b60991f92158afa4f4f02a5410d4791efccc92caaa166dc", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 192, ExternalTrackId = "629899792", Source = "Deezer", Title = "fake smile", ArtistId = 13, AlbumId = 14, DurationSeconds = 208, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/1/c/0/41c303dca49c6b266a437676dce41755.mp3?hdnea=exp=1776617628~acl=/api/1/1/4/1/c/0/41c303dca49c6b266a437676dce41755.mp3*~data=user_id=0,application_id=42~hmac=f243aee94e87811fe3cccfa4da972959ab2f3912658c958635cac062436bc2c5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 193, ExternalTrackId = "629899802", Source = "Deezer", Title = "bad idea", ArtistId = 13, AlbumId = 14, DurationSeconds = 266, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/d/c/0/adc35277924214d2e05daf4cb3ef76ba.mp3?hdnea=exp=1776617629~acl=/api/1/1/a/d/c/0/adc35277924214d2e05daf4cb3ef76ba.mp3*~data=user_id=0,application_id=42~hmac=b98ccc6d7a06ecc2c142fbb71fcb124de33cba2e52b689f529618d83a2a3e7ee", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 194, ExternalTrackId = "629899812", Source = "Deezer", Title = "make up", ArtistId = 13, AlbumId = 14, DurationSeconds = 140, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/7/4/0/37488af3f2f0b792931ad9df02ff3191.mp3?hdnea=exp=1776617630~acl=/api/1/1/3/7/4/0/37488af3f2f0b792931ad9df02ff3191.mp3*~data=user_id=0,application_id=42~hmac=8fb79ae9b763ac7d0631abcedb061d167d7a748b32a5ac19f2c689275aeb3832", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 195, ExternalTrackId = "629899822", Source = "Deezer", Title = "ghostin", ArtistId = 13, AlbumId = 14, DurationSeconds = 270, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/7/3/0/7736432b83780be0d53f3cfb678999a7.mp3?hdnea=exp=1776617630~acl=/api/1/1/7/7/3/0/7736432b83780be0d53f3cfb678999a7.mp3*~data=user_id=0,application_id=42~hmac=6fa675f4de5bf9721c80591be64bafea9270829e73d4c4da8668b57bf4580876", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 196, ExternalTrackId = "629899832", Source = "Deezer", Title = "in my head", ArtistId = 13, AlbumId = 14, DurationSeconds = 222, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/c/c/0/8cc5803c16cf91b82246dc1fc7fa7412.mp3?hdnea=exp=1776617631~acl=/api/1/1/8/c/c/0/8cc5803c16cf91b82246dc1fc7fa7412.mp3*~data=user_id=0,application_id=42~hmac=d832aada63e63abf8484afff0d31b389143c4556dcf4816fecc9a3e004d0f2bb", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 197, ExternalTrackId = "629899842", Source = "Deezer", Title = "7 rings", ArtistId = 13, AlbumId = 14, DurationSeconds = 178, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/2/2/0/e2210dc9ffd5633719e4f695d42e4554.mp3?hdnea=exp=1776617632~acl=/api/1/1/e/2/2/0/e2210dc9ffd5633719e4f695d42e4554.mp3*~data=user_id=0,application_id=42~hmac=659b4cc541ab550a30be12af6117538797f758a4683bfaa76ef31d6a1473dcc1", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 198, ExternalTrackId = "629899852", Source = "Deezer", Title = "thank u, next", ArtistId = 13, AlbumId = 14, DurationSeconds = 206, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/e/f/0/eefa4d026d664cfa4fdc8c74a3d6edf4.mp3?hdnea=exp=1776617632~acl=/api/1/1/e/e/f/0/eefa4d026d664cfa4fdc8c74a3d6edf4.mp3*~data=user_id=0,application_id=42~hmac=b89f47535d17a52bdf259b619e928b45e36bcbc7f0fd727e975c97ba0765a56b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 199, ExternalTrackId = "629899862", Source = "Deezer", Title = "break up with your girlfriend, i'm bored", ArtistId = 13, AlbumId = 14, DurationSeconds = 189, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/a/a/0/caaa1d42d52f6352516cc48250067be2.mp3?hdnea=exp=1776617633~acl=/api/1/1/c/a/a/0/caaa1d42d52f6352516cc48250067be2.mp3*~data=user_id=0,application_id=42~hmac=9957d8e3395c23333e6e8dcf446feb81cf2805a8b7fdfea5802e61cf08d8f0a1", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 200, ExternalTrackId = "62376283", Source = "Deezer", Title = "Fresh Out The Runway", ArtistId = 14, AlbumId = 15, DurationSeconds = 224, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/a/9/0/da90b6ebed24277ab0927439042e6f84.mp3?hdnea=exp=1776617634~acl=/api/1/1/d/a/9/0/da90b6ebed24277ab0927439042e6f84.mp3*~data=user_id=0,application_id=42~hmac=bceb4cec64dccf4e076a93199c3939d54c94aa814a76d20ae0a658acdfea281d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 201, ExternalTrackId = "62376284", Source = "Deezer", Title = "Diamonds", ArtistId = 14, AlbumId = 15, DurationSeconds = 225, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/f/1/0/6f1c238e57ae216d9e30ae712b6ccb88.mp3?hdnea=exp=1776617634~acl=/api/1/1/6/f/1/0/6f1c238e57ae216d9e30ae712b6ccb88.mp3*~data=user_id=0,application_id=42~hmac=8a5fa0620268b1776301fd2852e2c5ce33b22792c7e1cf696ea54833043d25e9", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 202, ExternalTrackId = "62376285", Source = "Deezer", Title = "Numb", ArtistId = 14, AlbumId = 15, DurationSeconds = 205, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/4/3/0/643ddea761c93f2d3231600c3d1a8683.mp3?hdnea=exp=1776617635~acl=/api/1/1/6/4/3/0/643ddea761c93f2d3231600c3d1a8683.mp3*~data=user_id=0,application_id=42~hmac=d1b028a3e6e4a3a6c23cc1dc8f54d474723e021633a378e78f771ca053603793", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 203, ExternalTrackId = "62376286", Source = "Deezer", Title = "Pour It Up", ArtistId = 14, AlbumId = 15, DurationSeconds = 161, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/9/9/0/b9994b67943e3d1148caf50a43048f42.mp3?hdnea=exp=1776617635~acl=/api/1/1/b/9/9/0/b9994b67943e3d1148caf50a43048f42.mp3*~data=user_id=0,application_id=42~hmac=ee1b02ea1fa77420f12d9d6a4d7cf81257614f1c5ab2e3e1b8396e995870b2e9", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 204, ExternalTrackId = "62376287", Source = "Deezer", Title = "Loveeeeeee Song", ArtistId = 14, AlbumId = 15, DurationSeconds = 256, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/e/f/0/0ef091325d3afe503114afab1c86dd25.mp3?hdnea=exp=1776617636~acl=/api/1/1/0/e/f/0/0ef091325d3afe503114afab1c86dd25.mp3*~data=user_id=0,application_id=42~hmac=8df9eccca7be7d3b55e686ab04a994c027ecc5f7b0d324a0d9efbd31f1f142d2", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 205, ExternalTrackId = "62376288", Source = "Deezer", Title = "Jump", ArtistId = 14, AlbumId = 15, DurationSeconds = 264, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/5/7/0/257ba51e0eaf262e6554d5418487ce73.mp3?hdnea=exp=1776617637~acl=/api/1/1/2/5/7/0/257ba51e0eaf262e6554d5418487ce73.mp3*~data=user_id=0,application_id=42~hmac=1577ceb80e92a2ee6469b21f02e7070f77a5b9791efdeeed3a94407ef0c2c210", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 206, ExternalTrackId = "62376289", Source = "Deezer", Title = "Right Now", ArtistId = 14, AlbumId = 15, DurationSeconds = 182, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/9/3/0/7935ee283b5892ab5b4f7bd68d2208b3.mp3?hdnea=exp=1776617637~acl=/api/1/1/7/9/3/0/7935ee283b5892ab5b4f7bd68d2208b3.mp3*~data=user_id=0,application_id=42~hmac=b0093a300e68c2a9bb9433727c77c2409702a14551a70606a4d69fb4b2697bb4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 207, ExternalTrackId = "62376290", Source = "Deezer", Title = "What Now", ArtistId = 14, AlbumId = 15, DurationSeconds = 243, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/8/d/0/e8d514fc5737439e31f9c61b54b04ecc.mp3?hdnea=exp=1776617638~acl=/api/1/1/e/8/d/0/e8d514fc5737439e31f9c61b54b04ecc.mp3*~data=user_id=0,application_id=42~hmac=1b2a2824096ad954e00857ba8c9910720f17527d5e7dc9f41511c607a66d2e6b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 208, ExternalTrackId = "62376291", Source = "Deezer", Title = "Stay", ArtistId = 14, AlbumId = 15, DurationSeconds = 241, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/2/4/0/024fa2b07b9cf44a902aa11ad7494cde.mp3?hdnea=exp=1776617638~acl=/api/1/1/0/2/4/0/024fa2b07b9cf44a902aa11ad7494cde.mp3*~data=user_id=0,application_id=42~hmac=7b405639572269c927677eccb9c2e99adec3136adfdfeb2a10a797037bec8939", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 209, ExternalTrackId = "62376292", Source = "Deezer", Title = "Nobody's Business", ArtistId = 14, AlbumId = 15, DurationSeconds = 216, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/7/7/0/477cb944bbe7442653fc3be70b8df470.mp3?hdnea=exp=1776617639~acl=/api/1/1/4/7/7/0/477cb944bbe7442653fc3be70b8df470.mp3*~data=user_id=0,application_id=42~hmac=d136114c19ea014a7cf7b59fb417dfb9fa29dee81d4ad8a0a0bcdfd571384f9f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 210, ExternalTrackId = "62376293", Source = "Deezer", Title = "Love Without Tragedy / Mother Mary", ArtistId = 14, AlbumId = 15, DurationSeconds = 418, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/5/5/0/9555093ee3a3b431b8c0d5cf147e46c0.mp3?hdnea=exp=1776617640~acl=/api/1/1/9/5/5/0/9555093ee3a3b431b8c0d5cf147e46c0.mp3*~data=user_id=0,application_id=42~hmac=6dbdd3f07ac3a996966b7080b0b42acd5b6d8fc198fa2432826715cb91c504ff", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 211, ExternalTrackId = "62376294", Source = "Deezer", Title = "Get It Over With", ArtistId = 14, AlbumId = 15, DurationSeconds = 211, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/2/9/0/929a256dd41091b5d2739a7ca4575c63.mp3?hdnea=exp=1776617640~acl=/api/1/1/9/2/9/0/929a256dd41091b5d2739a7ca4575c63.mp3*~data=user_id=0,application_id=42~hmac=5e94ec0bc2f3366a6cf3518e2272a321136e99f84043ab04b4206b36fef3c3d9", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 212, ExternalTrackId = "62376295", Source = "Deezer", Title = "No Love Allowed", ArtistId = 14, AlbumId = 15, DurationSeconds = 249, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/0/e/0/60eea5042275b99dd525b250819b95bb.mp3?hdnea=exp=1776617641~acl=/api/1/1/6/0/e/0/60eea5042275b99dd525b250819b95bb.mp3*~data=user_id=0,application_id=42~hmac=aaa98106df6d731231e43e4aef53f9229e43157d2b3a80c10bbe96582e32b09f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 213, ExternalTrackId = "62376296", Source = "Deezer", Title = "Lost In Paradise", ArtistId = 14, AlbumId = 15, DurationSeconds = 216, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/0/5/0/405384b20631b60d88c16926b6d0f669.mp3?hdnea=exp=1776617642~acl=/api/1/1/4/0/5/0/405384b20631b60d88c16926b6d0f669.mp3*~data=user_id=0,application_id=42~hmac=4d6dbde5108b4ada378988ced196a4b3ba62ba081278ddd11e7381cc63f5c1ff", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 214, ExternalTrackId = "62376297", Source = "Deezer", Title = "Half Of Me", ArtistId = 14, AlbumId = 15, DurationSeconds = 192, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/4/4/0/1448d6adc3066efaec8eb56cc72e92b3.mp3?hdnea=exp=1776617643~acl=/api/1/1/1/4/4/0/1448d6adc3066efaec8eb56cc72e92b3.mp3*~data=user_id=0,application_id=42~hmac=8d7e2f59152a9a42c27bd05171a15d59e4a1b1d5cfffe3b8ee0a74a8e57f05f4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 215, ExternalTrackId = "62376298", Source = "Deezer", Title = "Diamonds (Dave Aude 100 Extended)", ArtistId = 14, AlbumId = 15, DurationSeconds = 302, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/7/7/0/e775b63d681cdf7aee87239686c219b2.mp3?hdnea=exp=1776617643~acl=/api/1/1/e/7/7/0/e775b63d681cdf7aee87239686c219b2.mp3*~data=user_id=0,application_id=42~hmac=50cc0fa32eb57052d4b0a0f62e83940e2bf758ab7455b0a4c5267d0165e54fca", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 216, ExternalTrackId = "62376299", Source = "Deezer", Title = "Diamonds (Gregor Salto Downtempo Remix)", ArtistId = 14, AlbumId = 15, DurationSeconds = 269, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/f/2/0/df2ba0de7e189e4cc6ba918c94a79211.mp3?hdnea=exp=1776617644~acl=/api/1/1/d/f/2/0/df2ba0de7e189e4cc6ba918c94a79211.mp3*~data=user_id=0,application_id=42~hmac=6978eb7285f377e8fd78f92204a16a58cd2e1b259d301dbbf3bb658496a150f9", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 217, ExternalTrackId = "13529559", Source = "Deezer", Title = "S&M", ArtistId = 14, AlbumId = 16, DurationSeconds = 243, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/1/e/0/21e9019453ca68928cbbbddcffb082d3.mp3?hdnea=exp=1776617644~acl=/api/1/1/2/1/e/0/21e9019453ca68928cbbbddcffb082d3.mp3*~data=user_id=0,application_id=42~hmac=19a3f7bb8a66640600c3a9b9d1b653007176b6baf043525d9ccf1768687dda11", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 218, ExternalTrackId = "13529560", Source = "Deezer", Title = "What's My Name? (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 264, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/e/d/0/eed9b5164512b0a0df27e4043fb7f57c.mp3?hdnea=exp=1776617645~acl=/api/1/1/e/e/d/0/eed9b5164512b0a0df27e4043fb7f57c.mp3*~data=user_id=0,application_id=42~hmac=6d870ecc654be67ccc9c73e11e9b0da9db4b10142c3e52b78363a831f6d30d6b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 219, ExternalTrackId = "13529561", Source = "Deezer", Title = "Cheers (Drink To That) (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 261, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/b/8/0/1b8f8f1d0adcb3665340f62224ac7bbc.mp3?hdnea=exp=1776617646~acl=/api/1/1/1/b/8/0/1b8f8f1d0adcb3665340f62224ac7bbc.mp3*~data=user_id=0,application_id=42~hmac=b2c96c2048e0c9e0fa3794b44fe4d3dd522babc90b0c57dcac472e908075b0d6", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 220, ExternalTrackId = "13529562", Source = "Deezer", Title = "Fading (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 207, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/d/9/0/fd9579c5d1f5ffab44344b820a4bfb3b.mp3?hdnea=exp=1776617646~acl=/api/1/1/f/d/9/0/fd9579c5d1f5ffab44344b820a4bfb3b.mp3*~data=user_id=0,application_id=42~hmac=3f3b10ed229667b1cc9d1277d6bce11bfea61e170db74c0ae4f8464e2e3fc602", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 221, ExternalTrackId = "13529563", Source = "Deezer", Title = "Only Girl (In The World)", ArtistId = 14, AlbumId = 16, DurationSeconds = 235, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/d/1/0/3d19eee9be5382b0e93b98a55a16c507.mp3?hdnea=exp=1776617647~acl=/api/1/1/3/d/1/0/3d19eee9be5382b0e93b98a55a16c507.mp3*~data=user_id=0,application_id=42~hmac=62888bba3e01bfea31d99166bf75794bba3378b1f0f5e548e544d0067758159c", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 222, ExternalTrackId = "13529564", Source = "Deezer", Title = "California King Bed (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 251, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/b/e/0/0be7c9c682a0fdc61999ef88a1a65631.mp3?hdnea=exp=1776617647~acl=/api/1/1/0/b/e/0/0be7c9c682a0fdc61999ef88a1a65631.mp3*~data=user_id=0,application_id=42~hmac=b0c48d500a313b0a4506827521d465820044d72dba8dd6b57f5e9768d987322e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 223, ExternalTrackId = "13529565", Source = "Deezer", Title = "Man Down (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 267, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/9/e/0/99e6b2e6800ba59fd4ef78793f8a76c6.mp3?hdnea=exp=1776617648~acl=/api/1/1/9/9/e/0/99e6b2e6800ba59fd4ef78793f8a76c6.mp3*~data=user_id=0,application_id=42~hmac=aecf3817f90560891469e57db9cf9d1b2d0caa18210a72a7f5995ca0833ef81c", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 224, ExternalTrackId = "13529566", Source = "Deezer", Title = "Raining Men (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 224, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/7/7/0/677df122a6fdfeec21fe9452ac5c51bb.mp3?hdnea=exp=1776617649~acl=/api/1/1/6/7/7/0/677df122a6fdfeec21fe9452ac5c51bb.mp3*~data=user_id=0,application_id=42~hmac=d1fd314989d60dfc46ccf288c51f678df5c7502663bd0dd651107144b959fb83", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 225, ExternalTrackId = "13529567", Source = "Deezer", Title = "Complicated (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 257, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/6/c/0/a6c5ee1193a3e2ca949af863ec47ce43.mp3?hdnea=exp=1776617649~acl=/api/1/1/a/6/c/0/a6c5ee1193a3e2ca949af863ec47ce43.mp3*~data=user_id=0,application_id=42~hmac=c23abcbd2db5ea7ca1001f1e844e0f48e826200e54fbf36b8a3b32fafd6f214a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 226, ExternalTrackId = "13529568", Source = "Deezer", Title = "Skin (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 303, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/2/9/0/e2912502d0fd925fd280970d7fbc1e4f.mp3?hdnea=exp=1776617650~acl=/api/1/1/e/2/9/0/e2912502d0fd925fd280970d7fbc1e4f.mp3*~data=user_id=0,application_id=42~hmac=acd5cb5b38f8807f18b219cb434476b0051ef41891a51d84e454e7f537ce1ee0", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 227, ExternalTrackId = "13529569", Source = "Deezer", Title = "Love The Way You Lie (Part II)", ArtistId = 14, AlbumId = 16, DurationSeconds = 296, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/9/a/0/39af0ddf19c0ae5825eb52c9d2cb89d7.mp3?hdnea=exp=1776617650~acl=/api/1/1/3/9/a/0/39af0ddf19c0ae5825eb52c9d2cb89d7.mp3*~data=user_id=0,application_id=42~hmac=91b8494a23fe30ae863720fc12c0889bf2cb51f460e0cc1637fbe6d4bd06b646", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 228, ExternalTrackId = "126772729", Source = "Deezer", Title = "Ultralight Beam", ArtistId = 4, AlbumId = 17, DurationSeconds = 320, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/d/9/0/4d9cd4b3eebf0e8eb35ad352582f6234.mp3?hdnea=exp=1776617651~acl=/api/1/1/4/d/9/0/4d9cd4b3eebf0e8eb35ad352582f6234.mp3*~data=user_id=0,application_id=42~hmac=53d719cd8b0d3629f74d8c45cd0c1590c954e4b5af8115d88c7fba8d60479ee8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 229, ExternalTrackId = "126772731", Source = "Deezer", Title = "Father Stretch My Hands Pt. 1", ArtistId = 4, AlbumId = 17, DurationSeconds = 135, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/b/6/0/0b68f7d18d33ebae98b6ceb257a3fc8f.mp3?hdnea=exp=1776617652~acl=/api/1/1/0/b/6/0/0b68f7d18d33ebae98b6ceb257a3fc8f.mp3*~data=user_id=0,application_id=42~hmac=2e76a4258a3a8960c854c808f9ac5cc766d9828263357a64bdec5564e108fcf9", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 230, ExternalTrackId = "126772733", Source = "Deezer", Title = "Pt. 2", ArtistId = 4, AlbumId = 17, DurationSeconds = 130, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/8/e/0/d8ef2b4d2a1e4b8a2673a8f2aa2b654b.mp3?hdnea=exp=1776617652~acl=/api/1/1/d/8/e/0/d8ef2b4d2a1e4b8a2673a8f2aa2b654b.mp3*~data=user_id=0,application_id=42~hmac=18549ff6355aba3920d6faebc1314219139a4ada314c77a25d790564e6aa6a80", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 231, ExternalTrackId = "126772735", Source = "Deezer", Title = "Famous", ArtistId = 4, AlbumId = 17, DurationSeconds = 192, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/5/6/0/4562c6160749e5b1b3fc961f8d5db494.mp3?hdnea=exp=1776617653~acl=/api/1/1/4/5/6/0/4562c6160749e5b1b3fc961f8d5db494.mp3*~data=user_id=0,application_id=42~hmac=c42b28957a0d0676e2547d5429efabb7685587e94329234370d56c58e1436b2b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 232, ExternalTrackId = "126772737", Source = "Deezer", Title = "Feedback", ArtistId = 4, AlbumId = 17, DurationSeconds = 147, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/1/3/0/a13fcf64a9992b197d5d007f0256c896.mp3?hdnea=exp=1776617654~acl=/api/1/1/a/1/3/0/a13fcf64a9992b197d5d007f0256c896.mp3*~data=user_id=0,application_id=42~hmac=3586f46301548989627a42d49e8298d987b1bafa2c40dbb2c9f94f00025abb99", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 233, ExternalTrackId = "126772739", Source = "Deezer", Title = "Low Lights", ArtistId = 4, AlbumId = 17, DurationSeconds = 131, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/7/3/0/773db103bd02ebf1e3d37677b889aaa6.mp3?hdnea=exp=1776617654~acl=/api/1/1/7/7/3/0/773db103bd02ebf1e3d37677b889aaa6.mp3*~data=user_id=0,application_id=42~hmac=4ceca99f5e4ea65bda741792d213764670c8cccd02880dda5de2a458d2a89893", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 234, ExternalTrackId = "126772741", Source = "Deezer", Title = "Highlights", ArtistId = 4, AlbumId = 17, DurationSeconds = 199, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/7/8/0/978a180229ff31c4b58a01dcacedc21b.mp3?hdnea=exp=1776617655~acl=/api/1/1/9/7/8/0/978a180229ff31c4b58a01dcacedc21b.mp3*~data=user_id=0,application_id=42~hmac=7427e80f03d7d859c17bb016a080fbc9c894160e1a9d62af4482ec0e1a3d5f83", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 235, ExternalTrackId = "126772743", Source = "Deezer", Title = "Freestyle 4", ArtistId = 4, AlbumId = 17, DurationSeconds = 123, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/e/5/0/ce5b6005243dd261ef78c1f9e98494fe.mp3?hdnea=exp=1776617655~acl=/api/1/1/c/e/5/0/ce5b6005243dd261ef78c1f9e98494fe.mp3*~data=user_id=0,application_id=42~hmac=e1672e3d8113d9264ffa7d4ca82e84202013740ae02868b80b2d5fa1660ba6cb", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 236, ExternalTrackId = "126772745", Source = "Deezer", Title = "I Love Kanye", ArtistId = 4, AlbumId = 17, DurationSeconds = 44, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/6/2/0/662ce3a0e9b1e86d70a5e336105361fb.mp3?hdnea=exp=1776617656~acl=/api/1/1/6/6/2/0/662ce3a0e9b1e86d70a5e336105361fb.mp3*~data=user_id=0,application_id=42~hmac=dc476e82d376ab1cd8d5ba62f53ceea861c33b38afecaa7ce84f924658cef07f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 237, ExternalTrackId = "126772747", Source = "Deezer", Title = "Waves", ArtistId = 4, AlbumId = 17, DurationSeconds = 181, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/8/7/0/18763d113a27a346164df56035dd701d.mp3?hdnea=exp=1776617657~acl=/api/1/1/1/8/7/0/18763d113a27a346164df56035dd701d.mp3*~data=user_id=0,application_id=42~hmac=95264cef4fd0d669c7a58c004b471551d6b675e0bf58644dd41ef1b926ca7ec8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 238, ExternalTrackId = "126772749", Source = "Deezer", Title = "FML", ArtistId = 4, AlbumId = 17, DurationSeconds = 236, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/4/4/0/a440826f7a08bf301f35b73a034875fe.mp3?hdnea=exp=1776617657~acl=/api/1/1/a/4/4/0/a440826f7a08bf301f35b73a034875fe.mp3*~data=user_id=0,application_id=42~hmac=aefe45e7d8c9089230ccf73b2bd1988460712c1ab6fa58e987eaab75541b6595", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 239, ExternalTrackId = "126772751", Source = "Deezer", Title = "Real Friends", ArtistId = 4, AlbumId = 17, DurationSeconds = 251, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/7/7/0/577aecd4e6b9253bc276e984ebeebadf.mp3?hdnea=exp=1776617658~acl=/api/1/1/5/7/7/0/577aecd4e6b9253bc276e984ebeebadf.mp3*~data=user_id=0,application_id=42~hmac=84403906973b325c7e0bda1c2fb8bce83b6c0d6435c05b8809242bbc86fa58fe", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 240, ExternalTrackId = "126772753", Source = "Deezer", Title = "Wolves", ArtistId = 4, AlbumId = 17, DurationSeconds = 301, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/6/9/0/d69d1d06aa745627496102f49de770b7.mp3?hdnea=exp=1776617659~acl=/api/1/1/d/6/9/0/d69d1d06aa745627496102f49de770b7.mp3*~data=user_id=0,application_id=42~hmac=7ed732421bae36bbefdd8f2068d97295bdb748cb43cf34a53b4ba169d99d16d3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 241, ExternalTrackId = "126772755", Source = "Deezer", Title = "Frank's Track", ArtistId = 4, AlbumId = 17, DurationSeconds = 38, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/f/5/0/9f50ecb848181e903ad469bb854110e7.mp3?hdnea=exp=1776617659~acl=/api/1/1/9/f/5/0/9f50ecb848181e903ad469bb854110e7.mp3*~data=user_id=0,application_id=42~hmac=a508f4888269ea81477040d446de3400d7f2ed15259f178e3983df05e9c0c2ce", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 242, ExternalTrackId = "126772757", Source = "Deezer", Title = "Siiiiiiiiilver Surffffeeeeer Intermission", ArtistId = 4, AlbumId = 17, DurationSeconds = 56, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/4/0/0/64099b7f272c626ce0da571bccf57c4d.mp3?hdnea=exp=1776617660~acl=/api/1/1/6/4/0/0/64099b7f272c626ce0da571bccf57c4d.mp3*~data=user_id=0,application_id=42~hmac=997d290be7a6ad4a4e52721b198d01e0f529408c6a6411e612e350f5fb90e620", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 243, ExternalTrackId = "126772759", Source = "Deezer", Title = "30 Hours", ArtistId = 4, AlbumId = 17, DurationSeconds = 323, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/4/e/0/f4e307874d1108a81fdb640d2b5d4f95.mp3?hdnea=exp=1776617660~acl=/api/1/1/f/4/e/0/f4e307874d1108a81fdb640d2b5d4f95.mp3*~data=user_id=0,application_id=42~hmac=e2b0e4d1dd8eb68299c1f5cf71a0518056f8fb4828b98350ed6fc10a36f21077", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 244, ExternalTrackId = "126772761", Source = "Deezer", Title = "No More Parties In LA", ArtistId = 4, AlbumId = 17, DurationSeconds = 374, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/6/f/0/b6fb6664db0d50a883a8e2aa828e28b9.mp3?hdnea=exp=1776617661~acl=/api/1/1/b/6/f/0/b6fb6664db0d50a883a8e2aa828e28b9.mp3*~data=user_id=0,application_id=42~hmac=e92365936f7654c141b72b05111043b6970faa64ddb3af2516ce7e2ca9b621ec", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 245, ExternalTrackId = "126772763", Source = "Deezer", Title = "Facts (Charlie Heat Version)", ArtistId = 4, AlbumId = 17, DurationSeconds = 200, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/4/0/0/6407280394f163f871f9b4c94c6677f0.mp3?hdnea=exp=1776617662~acl=/api/1/1/6/4/0/0/6407280394f163f871f9b4c94c6677f0.mp3*~data=user_id=0,application_id=42~hmac=cb6440c096f378d7efef7c0ce7edf96349b8b571eec149a4990caaeaee8e199b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 246, ExternalTrackId = "126772765", Source = "Deezer", Title = "Fade", ArtistId = 4, AlbumId = 17, DurationSeconds = 193, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/d/a/0/8da28a2f07ce33ed8ec5cff1a9fec652.mp3?hdnea=exp=1776617662~acl=/api/1/1/8/d/a/0/8da28a2f07ce33ed8ec5cff1a9fec652.mp3*~data=user_id=0,application_id=42~hmac=5f52622e8eb81421b99111ba9b93a64edfcbc4d604697bb72d02641e8babd65d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 247, ExternalTrackId = "126772767", Source = "Deezer", Title = "Saint Pablo", ArtistId = 4, AlbumId = 17, DurationSeconds = 372, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/7/1/0/87188b90de7f795025ff4e921896bd9a.mp3?hdnea=exp=1776617663~acl=/api/1/1/8/7/1/0/87188b90de7f795025ff4e921896bd9a.mp3*~data=user_id=0,application_id=42~hmac=d4fb951b841f9e861bf6c503ddbc5bdb916495c03f1aec68bd144a1ef2de8cb7", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 248, ExternalTrackId = "725929", Source = "Deezer", Title = "By the Way", ArtistId = 15, AlbumId = 18, DurationSeconds = 216, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/f/5/0/af5f50e398ed52b65b2eab10c0cc9884.mp3?hdnea=exp=1776617663~acl=/api/1/1/a/f/5/0/af5f50e398ed52b65b2eab10c0cc9884.mp3*~data=user_id=0,application_id=42~hmac=6215d5bb9435a229c44e5baa7a00dbb7adcde3d6d130ec740f63f226fbe715b8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 249, ExternalTrackId = "725937", Source = "Deezer", Title = "Universally Speaking", ArtistId = 15, AlbumId = 18, DurationSeconds = 256, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/7/f/0/47f4f9d10c597bc50e060fd314237a2a.mp3?hdnea=exp=1776617664~acl=/api/1/1/4/7/f/0/47f4f9d10c597bc50e060fd314237a2a.mp3*~data=user_id=0,application_id=42~hmac=7ae3e7e25e19cff6b79dda65fcac1f9f2245fac1e0916afdf772606cbb2d850c", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 250, ExternalTrackId = "725943", Source = "Deezer", Title = "This Is the Place", ArtistId = 15, AlbumId = 18, DurationSeconds = 257, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/c/9/0/cc92c568c3147ea618dd37b6dea74907.mp3?hdnea=exp=1776617665~acl=/api/1/1/c/c/9/0/cc92c568c3147ea618dd37b6dea74907.mp3*~data=user_id=0,application_id=42~hmac=a2f5cd558b704e421280d9c82404dd7d10e406a49bc9483488359232838df8af", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 251, ExternalTrackId = "725950", Source = "Deezer", Title = "Dosed", ArtistId = 15, AlbumId = 18, DurationSeconds = 311, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/d/b/0/0dbb03ddf42674890a6c9d1a7cf0de1a.mp3?hdnea=exp=1776617665~acl=/api/1/1/0/d/b/0/0dbb03ddf42674890a6c9d1a7cf0de1a.mp3*~data=user_id=0,application_id=42~hmac=d99a33bf309ed80d299070d973888f8b8651feb9698f17020f70ac1a861bafc0", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 252, ExternalTrackId = "725959", Source = "Deezer", Title = "Don't Forget Me", ArtistId = 15, AlbumId = 18, DurationSeconds = 277, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/1/b/0/d1b55bfca3be26c59f0b4c633bc841f1.mp3?hdnea=exp=1776617666~acl=/api/1/1/d/1/b/0/d1b55bfca3be26c59f0b4c633bc841f1.mp3*~data=user_id=0,application_id=42~hmac=33b3c4c41b6f2f4ee8d9ee79bc30582c22f738166987b4a55ad876cf98d73fd0", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 253, ExternalTrackId = "725965", Source = "Deezer", Title = "The Zephyr Song", ArtistId = 15, AlbumId = 18, DurationSeconds = 231, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/e/d/0/4edc45d7a59f443f56080a5c57c84fcc.mp3?hdnea=exp=1776617667~acl=/api/1/1/4/e/d/0/4edc45d7a59f443f56080a5c57c84fcc.mp3*~data=user_id=0,application_id=42~hmac=8677a846fb8fd27d7c1afc4298f5e31a6c18c23f6be97ba0f1d0a88b6be65875", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 254, ExternalTrackId = "725971", Source = "Deezer", Title = "Can't Stop", ArtistId = 15, AlbumId = 18, DurationSeconds = 269, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/f/c/0/ffc7c5e75630f75d1b164d9ec37838e3.mp3?hdnea=exp=1776617667~acl=/api/1/1/f/f/c/0/ffc7c5e75630f75d1b164d9ec37838e3.mp3*~data=user_id=0,application_id=42~hmac=533967b1c81e61e2a93990edb0376d55e2ad261d7acaf273dbdf86b0abb2b81a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 255, ExternalTrackId = "725976", Source = "Deezer", Title = "I Could Die for You", ArtistId = 15, AlbumId = 18, DurationSeconds = 192, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/1/1/0/1114b86069166b30edcbcd053c613cc5.mp3?hdnea=exp=1776617668~acl=/api/1/1/1/1/1/0/1114b86069166b30edcbcd053c613cc5.mp3*~data=user_id=0,application_id=42~hmac=00bcb4c7696d8b6fbc805c0acc24756d14492d3d1cb453cbeef0a157a612b735", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 256, ExternalTrackId = "725984", Source = "Deezer", Title = "Midnight", ArtistId = 15, AlbumId = 18, DurationSeconds = 295, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/a/b/0/fab8ba47150a48a4d3e2890a8f0c1187.mp3?hdnea=exp=1776617668~acl=/api/1/1/f/a/b/0/fab8ba47150a48a4d3e2890a8f0c1187.mp3*~data=user_id=0,application_id=42~hmac=ac829bc33d8409be98c64d05df0a04c344c5ffc5cac91934f9b24f36a9537976", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 257, ExternalTrackId = "725989", Source = "Deezer", Title = "Throw Away Your Television", ArtistId = 15, AlbumId = 18, DurationSeconds = 224, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/d/8/0/fd8fcbb5382761169201b640a6cb0b90.mp3?hdnea=exp=1776617669~acl=/api/1/1/f/d/8/0/fd8fcbb5382761169201b640a6cb0b90.mp3*~data=user_id=0,application_id=42~hmac=185576232a777fa1b7edb45fe4303e7dca196e92f04881816f09fc95f4bca392", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 258, ExternalTrackId = "725995", Source = "Deezer", Title = "Cabron", ArtistId = 15, AlbumId = 18, DurationSeconds = 218, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/9/f/0/29f7bc915cffbbaf9d28fb4773785153.mp3?hdnea=exp=1776617670~acl=/api/1/1/2/9/f/0/29f7bc915cffbbaf9d28fb4773785153.mp3*~data=user_id=0,application_id=42~hmac=c38e8a846ab5a416d0f2291f33755abd4ea54427703b82eb01a6bdadaf6320bc", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 259, ExternalTrackId = "726003", Source = "Deezer", Title = "Tear", ArtistId = 15, AlbumId = 18, DurationSeconds = 317, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/c/1/0/bc14ce25a482010dc3105680e0914e34.mp3?hdnea=exp=1776617670~acl=/api/1/1/b/c/1/0/bc14ce25a482010dc3105680e0914e34.mp3*~data=user_id=0,application_id=42~hmac=426da73b1b18772fb653cf688cfb991c0972dc5cc424b87f2f43f5bd2247f77f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 260, ExternalTrackId = "726007", Source = "Deezer", Title = "On Mercury", ArtistId = 15, AlbumId = 18, DurationSeconds = 207, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/1/3/0/1139ee63b3136d6427c6e6065d659001.mp3?hdnea=exp=1776617671~acl=/api/1/1/1/1/3/0/1139ee63b3136d6427c6e6065d659001.mp3*~data=user_id=0,application_id=42~hmac=a0ad07127e0be4304d4f348752397ad63a37da1831c55f043a0b1c4728beed05", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 261, ExternalTrackId = "726014", Source = "Deezer", Title = "Minor Thing", ArtistId = 15, AlbumId = 18, DurationSeconds = 217, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/c/0/0/2c063fef2e6862caf00c774b06d3b71c.mp3?hdnea=exp=1776617671~acl=/api/1/1/2/c/0/0/2c063fef2e6862caf00c774b06d3b71c.mp3*~data=user_id=0,application_id=42~hmac=114b14ad5b5e043db664b133b4f723eff6f47326e567085e864e3207f0f8cd8e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 262, ExternalTrackId = "726021", Source = "Deezer", Title = "Warm Tape", ArtistId = 15, AlbumId = 18, DurationSeconds = 255, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/b/2/0/eb2219c2c430d11352030ffe25ab081f.mp3?hdnea=exp=1776617672~acl=/api/1/1/e/b/2/0/eb2219c2c430d11352030ffe25ab081f.mp3*~data=user_id=0,application_id=42~hmac=b57f2c498ee1e102210d1916c3bdf31be48ffbf65bd9d97b31abcc759e2624b4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 263, ExternalTrackId = "726027", Source = "Deezer", Title = "Venice Queen", ArtistId = 15, AlbumId = 18, DurationSeconds = 367, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/a/5/0/7a5a8c970ae309778ec590254a9e14cc.mp3?hdnea=exp=1776617673~acl=/api/1/1/7/a/5/0/7a5a8c970ae309778ec590254a9e14cc.mp3*~data=user_id=0,application_id=42~hmac=399c5b1db4751c0020edd1de384d8fe0846ee0700b18fd047981490d716aad21", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 264, ExternalTrackId = "726034", Source = "Deezer", Title = "Runaway (2006 Remaster)", ArtistId = 15, AlbumId = 18, DurationSeconds = 270, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/1/1/0/011822ead607c4f7e6d83164e1cabcc6.mp3?hdnea=exp=1776617673~acl=/api/1/1/0/1/1/0/011822ead607c4f7e6d83164e1cabcc6.mp3*~data=user_id=0,application_id=42~hmac=5a7d102fc6bf3da75de4c162bae045926b98b47160910fd6d277dc90a5bfada5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 265, ExternalTrackId = "726042", Source = "Deezer", Title = "Bicycle Song (2006 Remaster)", ArtistId = 15, AlbumId = 18, DurationSeconds = 203, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/b/f/0/5bfdc45ddd8db672876191ddfed83daf.mp3?hdnea=exp=1776617674~acl=/api/1/1/5/b/f/0/5bfdc45ddd8db672876191ddfed83daf.mp3*~data=user_id=0,application_id=42~hmac=eb86685a994c1574ce70b9cdc683988968416083be539c74c85435a5136befcb", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 266, ExternalTrackId = "680516", Source = "Deezer", Title = "Dani California", ArtistId = 15, AlbumId = 19, DurationSeconds = 282, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/0/6/0/306de2a55d56aa17a8f5280eed398b43.mp3?hdnea=exp=1776617674~acl=/api/1/1/3/0/6/0/306de2a55d56aa17a8f5280eed398b43.mp3*~data=user_id=0,application_id=42~hmac=8283e5ac2c752f559b9d4266b59cceb5e149a81d4f7bd27c197c02e5bbea0868", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 267, ExternalTrackId = "680518", Source = "Deezer", Title = "Snow (Hey Oh)", ArtistId = 15, AlbumId = 19, DurationSeconds = 334, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/1/9/0/8199a2b66b31069b367d7fb0e2b31e61.mp3?hdnea=exp=1776617675~acl=/api/1/1/8/1/9/0/8199a2b66b31069b367d7fb0e2b31e61.mp3*~data=user_id=0,application_id=42~hmac=56cab1fffe9ebe8f2cfdda35f67bef54ce86943c4db19567f0118bec3ca16dbe", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 268, ExternalTrackId = "680520", Source = "Deezer", Title = "Charlie", ArtistId = 15, AlbumId = 19, DurationSeconds = 277, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/d/c/0/3dca00442b922c2c4f38c481b2a5a8f9.mp3?hdnea=exp=1776617675~acl=/api/1/1/3/d/c/0/3dca00442b922c2c4f38c481b2a5a8f9.mp3*~data=user_id=0,application_id=42~hmac=c8023a7a5bd7125f6f5bfed68698dd6c99a4bd425897bed56ae3e208f283ded6", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 269, ExternalTrackId = "680522", Source = "Deezer", Title = "Stadium Arcadium", ArtistId = 15, AlbumId = 19, DurationSeconds = 314, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/9/8/0/9981f7ea9784fe34d30d4a54e28823d1.mp3?hdnea=exp=1776617676~acl=/api/1/1/9/9/8/0/9981f7ea9784fe34d30d4a54e28823d1.mp3*~data=user_id=0,application_id=42~hmac=4f07f2e9f5617c8070a5a795a6bd34f9b48e7b2b134689722555588c51d1de8a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 270, ExternalTrackId = "680524", Source = "Deezer", Title = "Hump de Bump", ArtistId = 15, AlbumId = 19, DurationSeconds = 213, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/b/d/0/3bdea75775509d35f928c9c92f2753e4.mp3?hdnea=exp=1776617677~acl=/api/1/1/3/b/d/0/3bdea75775509d35f928c9c92f2753e4.mp3*~data=user_id=0,application_id=42~hmac=b64a40bbdc61acf6e8c3f009d4575424c7c8c6df34f6d2a0edb700dfe8e16788", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 271, ExternalTrackId = "680525", Source = "Deezer", Title = "She's Only 18", ArtistId = 15, AlbumId = 19, DurationSeconds = 205, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/6/0/0/460014ea73d5696531629fa4475ade87.mp3?hdnea=exp=1776617677~acl=/api/1/1/4/6/0/0/460014ea73d5696531629fa4475ade87.mp3*~data=user_id=0,application_id=42~hmac=a067c42b7e5d01d9c82e86da5b1c7c0b2feda66253d73e4a857b42142eaee169", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 272, ExternalTrackId = "680528", Source = "Deezer", Title = "Slow Cheetah", ArtistId = 15, AlbumId = 19, DurationSeconds = 319, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/f/c/0/ffc8fa2201445755cec41238013dafef.mp3?hdnea=exp=1776617678~acl=/api/1/1/f/f/c/0/ffc8fa2201445755cec41238013dafef.mp3*~data=user_id=0,application_id=42~hmac=d3c94468346fafdf509ece3e4ed3ac7ae55e2085cbd94a79135871195fd4e834", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 273, ExternalTrackId = "680529", Source = "Deezer", Title = "Torture Me", ArtistId = 15, AlbumId = 19, DurationSeconds = 224, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/0/6/0/50657a0fbd7f9bc5e0947a79ba2090d2.mp3?hdnea=exp=1776617679~acl=/api/1/1/5/0/6/0/50657a0fbd7f9bc5e0947a79ba2090d2.mp3*~data=user_id=0,application_id=42~hmac=de17946952479e28772bbc85f8705396fb25d5d0af74708a3e72c7311bbbc476", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 274, ExternalTrackId = "680532", Source = "Deezer", Title = "Strip My Mind", ArtistId = 15, AlbumId = 19, DurationSeconds = 259, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/c/b/0/4cb9f00113067c3d6e31a990e3bbae81.mp3?hdnea=exp=1776617679~acl=/api/1/1/4/c/b/0/4cb9f00113067c3d6e31a990e3bbae81.mp3*~data=user_id=0,application_id=42~hmac=3de8f70b8069c8447fc572aac8b3cef07e8aa94b70b5deb10668b6dd7beac9f0", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 275, ExternalTrackId = "680534", Source = "Deezer", Title = "Especially in Michigan", ArtistId = 15, AlbumId = 19, DurationSeconds = 240, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/6/f/0/66f5ae6eb293f7461ab193f60c94abee.mp3?hdnea=exp=1776617680~acl=/api/1/1/6/6/f/0/66f5ae6eb293f7461ab193f60c94abee.mp3*~data=user_id=0,application_id=42~hmac=22a492e66e36a63887f94792f679ddea75b4676ebf25f5befe1c4907f346564e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 276, ExternalTrackId = "680536", Source = "Deezer", Title = "Warlocks", ArtistId = 15, AlbumId = 19, DurationSeconds = 205, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/e/0/0/8e0525a5d1f40346adc38128cdca8ff2.mp3?hdnea=exp=1776617681~acl=/api/1/1/8/e/0/0/8e0525a5d1f40346adc38128cdca8ff2.mp3*~data=user_id=0,application_id=42~hmac=7952b5b471666f7abaefb69a03c5839d607f11f2a3ee6c498a5e506e06832c8f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 277, ExternalTrackId = "680537", Source = "Deezer", Title = "C'mon Girl", ArtistId = 15, AlbumId = 19, DurationSeconds = 228, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/6/1/0/a61965e302d46cf370d3d652801f578e.mp3?hdnea=exp=1776617681~acl=/api/1/1/a/6/1/0/a61965e302d46cf370d3d652801f578e.mp3*~data=user_id=0,application_id=42~hmac=33fbe996c27981faccd7a74b3e087a25de63a42d77d44e060fe69bbbe4cbf177", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 278, ExternalTrackId = "680538", Source = "Deezer", Title = "Wet Sand", ArtistId = 15, AlbumId = 19, DurationSeconds = 309, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/0/e/0/50ed3de1efd26b27faa4e903adc9973e.mp3?hdnea=exp=1776617682~acl=/api/1/1/5/0/e/0/50ed3de1efd26b27faa4e903adc9973e.mp3*~data=user_id=0,application_id=42~hmac=1d8d1e5e82e36aa74fdd6d854da233b595ff5bfff9a9a42f17a26f445da4a7d8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 279, ExternalTrackId = "680539", Source = "Deezer", Title = "Hey", ArtistId = 15, AlbumId = 19, DurationSeconds = 339, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/9/4/0/f9441ba73debc47a4427426a3fe345c1.mp3?hdnea=exp=1776617682~acl=/api/1/1/f/9/4/0/f9441ba73debc47a4427426a3fe345c1.mp3*~data=user_id=0,application_id=42~hmac=21cbb489b0c4f854b21319f023e73523c580bed5b9962a5f224f9b05370719a0", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 280, ExternalTrackId = "680540", Source = "Deezer", Title = "Desecration Smile", ArtistId = 15, AlbumId = 19, DurationSeconds = 301, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/5/9/0/8591ef66b52c8af0b72f873eae122a4b.mp3?hdnea=exp=1776617683~acl=/api/1/1/8/5/9/0/8591ef66b52c8af0b72f873eae122a4b.mp3*~data=user_id=0,application_id=42~hmac=57cd898c0a6f53742e63d2d230c314362fa3e5b4a8094b1cfefdcd0c52e528e1", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 281, ExternalTrackId = "680542", Source = "Deezer", Title = "Tell Me Baby", ArtistId = 15, AlbumId = 19, DurationSeconds = 247, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/6/c/0/f6c0a847c4c0dfa030c70bdc5bbd991d.mp3?hdnea=exp=1776617684~acl=/api/1/1/f/6/c/0/f6c0a847c4c0dfa030c70bdc5bbd991d.mp3*~data=user_id=0,application_id=42~hmac=4a12dbb4bbe1da7058538eb2dc515fcfe48636863e5bf99d1a13f81619889799", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 282, ExternalTrackId = "680544", Source = "Deezer", Title = "Hard to Concentrate", ArtistId = 15, AlbumId = 19, DurationSeconds = 241, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/4/e/0/84edd750b23c0a84a1c79fec0f19c789.mp3?hdnea=exp=1776617684~acl=/api/1/1/8/4/e/0/84edd750b23c0a84a1c79fec0f19c789.mp3*~data=user_id=0,application_id=42~hmac=88e241b95de4ed564b02042c525d055c9eb1fe050445e6857ef195ca2e29b643", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 283, ExternalTrackId = "680546", Source = "Deezer", Title = "21st Century", ArtistId = 15, AlbumId = 19, DurationSeconds = 262, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/1/5/0/61520f54004a06fc7f05a75046460f77.mp3?hdnea=exp=1776617685~acl=/api/1/1/6/1/5/0/61520f54004a06fc7f05a75046460f77.mp3*~data=user_id=0,application_id=42~hmac=40eebcd232c883f7886ab61c06bc0d9c39e3801a8be323cbdd8a334f8aa5c567", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 284, ExternalTrackId = "680548", Source = "Deezer", Title = "She Looks to Me", ArtistId = 15, AlbumId = 19, DurationSeconds = 245, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/f/a/0/3fa7d77db51961bfbe0a459f22e38b83.mp3?hdnea=exp=1776617686~acl=/api/1/1/3/f/a/0/3fa7d77db51961bfbe0a459f22e38b83.mp3*~data=user_id=0,application_id=42~hmac=63b3ea92c30725f483e836da7a47987820d777fd73060963d711ad2942355487", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 285, ExternalTrackId = "680550", Source = "Deezer", Title = "Readymade", ArtistId = 15, AlbumId = 19, DurationSeconds = 270, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/c/e/0/9ce909dab8f9985a26b65fc0eaa25259.mp3?hdnea=exp=1776617686~acl=/api/1/1/9/c/e/0/9ce909dab8f9985a26b65fc0eaa25259.mp3*~data=user_id=0,application_id=42~hmac=e48d8e6e3f27612064e034f930c08348df5aec41fed100d35356d9797d386a92", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 286, ExternalTrackId = "680552", Source = "Deezer", Title = "If", ArtistId = 15, AlbumId = 19, DurationSeconds = 172, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/8/1/0/481d44d33db2ab8f4c74ec196f6a5bcc.mp3?hdnea=exp=1776617687~acl=/api/1/1/4/8/1/0/481d44d33db2ab8f4c74ec196f6a5bcc.mp3*~data=user_id=0,application_id=42~hmac=f833255873640c813fb22932dc2234479ca4eab7e7254edf3914b518bace896e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 287, ExternalTrackId = "680555", Source = "Deezer", Title = "Make You Feel Better", ArtistId = 15, AlbumId = 19, DurationSeconds = 231, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/a/b/0/aab6506cef3ca91383c9afdd8ec82549.mp3?hdnea=exp=1776617687~acl=/api/1/1/a/a/b/0/aab6506cef3ca91383c9afdd8ec82549.mp3*~data=user_id=0,application_id=42~hmac=ed2c7dd87a39f981d87d5ad14f155940993cd9fd5b21e08043424330861212b1", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 288, ExternalTrackId = "680557", Source = "Deezer", Title = "Animal Bar", ArtistId = 15, AlbumId = 19, DurationSeconds = 325, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/f/f/0/0ff6ad724f24566991081f01e2f11df0.mp3?hdnea=exp=1776617688~acl=/api/1/1/0/f/f/0/0ff6ad724f24566991081f01e2f11df0.mp3*~data=user_id=0,application_id=42~hmac=a5b47d7e5b6b256962c43507d9f31f8835b406126d8741881537b8462406643f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 289, ExternalTrackId = "680559", Source = "Deezer", Title = "So Much I", ArtistId = 15, AlbumId = 19, DurationSeconds = 224, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/d/d/0/addf9bbde50af7a7ad48137139538705.mp3?hdnea=exp=1776617688~acl=/api/1/1/a/d/d/0/addf9bbde50af7a7ad48137139538705.mp3*~data=user_id=0,application_id=42~hmac=754621a80b0ac94a69338f010a30220a36b8aeb2fd2114069500feb6ce775f1b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 290, ExternalTrackId = "680560", Source = "Deezer", Title = "Storm in a Teacup", ArtistId = 15, AlbumId = 19, DurationSeconds = 224, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/1/d/0/d1d0fe9eadb110663dd2e9269c8a8809.mp3?hdnea=exp=1776617689~acl=/api/1/1/d/1/d/0/d1d0fe9eadb110663dd2e9269c8a8809.mp3*~data=user_id=0,application_id=42~hmac=473ca5919e31e64a08fa889023a30b5eddf935b2f3073fdff13d50c446a72e1f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 291, ExternalTrackId = "138539971", Source = "Deezer", Title = "Airbag", ArtistId = 16, AlbumId = 20, DurationSeconds = 287, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/3/e/0/03e8e4bba953f552380f0e8476e9394c.mp3?hdnea=exp=1776617690~acl=/api/1/1/0/3/e/0/03e8e4bba953f552380f0e8476e9394c.mp3*~data=user_id=0,application_id=42~hmac=eeb18817b9db616e1003b960ded9cc5d11ebaf6d01bf23660da4a63d6640d5ef", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 292, ExternalTrackId = "138539973", Source = "Deezer", Title = "Paranoid Android", ArtistId = 16, AlbumId = 20, DurationSeconds = 387, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/9/0/0/e906ec366652d0fe7af06d993e959f78.mp3?hdnea=exp=1776617690~acl=/api/1/1/e/9/0/0/e906ec366652d0fe7af06d993e959f78.mp3*~data=user_id=0,application_id=42~hmac=0e98c4e533810c28ef86aa5c6e252e658df4c1da989542cc8a8b9c2eba841ff1", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 293, ExternalTrackId = "138539975", Source = "Deezer", Title = "Subterranean Homesick Alien", ArtistId = 16, AlbumId = 20, DurationSeconds = 267, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/e/9/0/0e9ed75adeb0794ce2c32ef29dd3a096.mp3?hdnea=exp=1776617691~acl=/api/1/1/0/e/9/0/0e9ed75adeb0794ce2c32ef29dd3a096.mp3*~data=user_id=0,application_id=42~hmac=695f9627e721a389871377464ff13c9504c512fcb5787cdd67fdcf0b8e8e5e43", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 294, ExternalTrackId = "138539977", Source = "Deezer", Title = "Exit Music (For A Film)", ArtistId = 16, AlbumId = 20, DurationSeconds = 267, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/1/9/0/4190e7b3593fe2674b48d06955a46fb1.mp3?hdnea=exp=1776617692~acl=/api/1/1/4/1/9/0/4190e7b3593fe2674b48d06955a46fb1.mp3*~data=user_id=0,application_id=42~hmac=7cc260c1d1289c6439e87999c45daca1c6b06e0a349306b33d1f780e4aa75646", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 295, ExternalTrackId = "138539979", Source = "Deezer", Title = "Let Down", ArtistId = 16, AlbumId = 20, DurationSeconds = 299, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/9/1/0/991f911408c85213268ebf001476d6b6.mp3?hdnea=exp=1776617692~acl=/api/1/1/9/9/1/0/991f911408c85213268ebf001476d6b6.mp3*~data=user_id=0,application_id=42~hmac=7ba571f144c2636689974ef25eddeb790cedbbaf5f0a32663d1bc83c8b7d0237", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 296, ExternalTrackId = "138539981", Source = "Deezer", Title = "Karma Police", ArtistId = 16, AlbumId = 20, DurationSeconds = 264, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/1/d/0/41dd34fd7d334b1c55b6970ef6db0d2f.mp3?hdnea=exp=1776617693~acl=/api/1/1/4/1/d/0/41dd34fd7d334b1c55b6970ef6db0d2f.mp3*~data=user_id=0,application_id=42~hmac=e2ff3e9acee9f7dc04a441ccf06611c57d77469ffb3e77485b3c4bea8072c520", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 297, ExternalTrackId = "138539983", Source = "Deezer", Title = "Fitter Happier", ArtistId = 16, AlbumId = 20, DurationSeconds = 117, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/a/7/0/8a7a8e9d290669365160640d72a199b8.mp3?hdnea=exp=1776617693~acl=/api/1/1/8/a/7/0/8a7a8e9d290669365160640d72a199b8.mp3*~data=user_id=0,application_id=42~hmac=0fd7aa0d2036075a800f303ddff3510e6d9ae504071dec186e664ac55792213a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 298, ExternalTrackId = "138539985", Source = "Deezer", Title = "Electioneering", ArtistId = 16, AlbumId = 20, DurationSeconds = 230, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/7/7/0/47730331b132d1841622d090723886a0.mp3?hdnea=exp=1776617694~acl=/api/1/1/4/7/7/0/47730331b132d1841622d090723886a0.mp3*~data=user_id=0,application_id=42~hmac=244c24ce0fddba91c6750fb902756bf1a86586af06eaad53c7e99314dc7f9b0e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 299, ExternalTrackId = "138539987", Source = "Deezer", Title = "Climbing Up the Walls", ArtistId = 16, AlbumId = 20, DurationSeconds = 285, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/1/6/0/8163f604a588bcc202370a2b1c6339c2.mp3?hdnea=exp=1776617695~acl=/api/1/1/8/1/6/0/8163f604a588bcc202370a2b1c6339c2.mp3*~data=user_id=0,application_id=42~hmac=c580ba5ffca2cb5fab5d17f4e4840ad50e087ed51b5c8e14713fdcf9956a48fa", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 300, ExternalTrackId = "138539989", Source = "Deezer", Title = "No Surprises", ArtistId = 16, AlbumId = 20, DurationSeconds = 229, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/a/b/0/8ab9535dab371b9546c6839a4838ab6a.mp3?hdnea=exp=1776617695~acl=/api/1/1/8/a/b/0/8ab9535dab371b9546c6839a4838ab6a.mp3*~data=user_id=0,application_id=42~hmac=dc58db1780a4f2b4bdd95635364ddcc9c656d31d340e2a2fac12bdad85ff0c39", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 301, ExternalTrackId = "138539991", Source = "Deezer", Title = "Lucky", ArtistId = 16, AlbumId = 20, DurationSeconds = 258, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/c/9/0/fc9b82fb77c1301d1669afb38dd7fe4d.mp3?hdnea=exp=1776617696~acl=/api/1/1/f/c/9/0/fc9b82fb77c1301d1669afb38dd7fe4d.mp3*~data=user_id=0,application_id=42~hmac=d1af54095916c583b380c6da0bb24e31a7ded4336fb5661d3b5abf586603b870", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 302, ExternalTrackId = "138539993", Source = "Deezer", Title = "The Tourist", ArtistId = 16, AlbumId = 20, DurationSeconds = 326, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/6/e/0/f6e0d3a706058af667f53d230d6b9c97.mp3?hdnea=exp=1776617697~acl=/api/1/1/f/6/e/0/f6e0d3a706058af667f53d230d6b9c97.mp3*~data=user_id=0,application_id=42~hmac=3ae651884c82824f2122a4ca734a78c4236bc10da4c78962659395ba8ac1d703", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 303, ExternalTrackId = "81836818", Source = "Deezer", Title = "Divinity", ArtistId = 17, AlbumId = 21, DurationSeconds = 367, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/d/1/0/1d1cd4d256aa3eb0aed55eaf170073f2.mp3?hdnea=exp=1776617697~acl=/api/1/1/1/d/1/0/1d1cd4d256aa3eb0aed55eaf170073f2.mp3*~data=user_id=0,application_id=42~hmac=a1db770be7c9d6b3f488e0daca9ba1f0ff69ec7129fb39bf4c8177fa46fb0af7", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 304, ExternalTrackId = "81836820", Source = "Deezer", Title = "Sad Machine", ArtistId = 17, AlbumId = 21, DurationSeconds = 350, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/b/b/0/ebbce2d541b9a7b1b37bf0f4f9eb0c9b.mp3?hdnea=exp=1776617698~acl=/api/1/1/e/b/b/0/ebbce2d541b9a7b1b37bf0f4f9eb0c9b.mp3*~data=user_id=0,application_id=42~hmac=7765b15463d35e23e34df9f36a0fccfde8c2aa44b80e28bd39ab8bc9e908e2b8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 305, ExternalTrackId = "81836822", Source = "Deezer", Title = "Years Of War", ArtistId = 17, AlbumId = 21, DurationSeconds = 233, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/3/0/0/d30d87a82a50e3318330f69d224bf616.mp3?hdnea=exp=1776617698~acl=/api/1/1/d/3/0/0/d30d87a82a50e3318330f69d224bf616.mp3*~data=user_id=0,application_id=42~hmac=03a32abc96d9bfcba97a4051a102ece29388945ecb4b9f1840282d8cc85419d3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 306, ExternalTrackId = "81836824", Source = "Deezer", Title = "Flicker", ArtistId = 17, AlbumId = 21, DurationSeconds = 277, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/4/6/0/d4621b85acaefe74f67e2fcff6baee14.mp3?hdnea=exp=1776617699~acl=/api/1/1/d/4/6/0/d4621b85acaefe74f67e2fcff6baee14.mp3*~data=user_id=0,application_id=42~hmac=facb5e44cb0704fe89f5ede276a3ef892b1c457d0bdf06579a7b3372f34a4c49", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 307, ExternalTrackId = "81836826", Source = "Deezer", Title = "Fresh Static Snow", ArtistId = 17, AlbumId = 21, DurationSeconds = 359, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/3/8/0/638247073a9c971f0435ed6556f8056f.mp3?hdnea=exp=1776617700~acl=/api/1/1/6/3/8/0/638247073a9c971f0435ed6556f8056f.mp3*~data=user_id=0,application_id=42~hmac=87f29a77f06aaa598a015c27301683a5b35aa4da192bda42a1c3f589934c4ec8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 308, ExternalTrackId = "81836828", Source = "Deezer", Title = "Polygon Dust", ArtistId = 17, AlbumId = 21, DurationSeconds = 208, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/9/3/0/793adde7925379310c43c5664055f570.mp3?hdnea=exp=1776617700~acl=/api/1/1/7/9/3/0/793adde7925379310c43c5664055f570.mp3*~data=user_id=0,application_id=42~hmac=9e276e4a12a6576722d32fdac8d8e54705cd206409c00e087744a3bd6b3e1280", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 309, ExternalTrackId = "81836830", Source = "Deezer", Title = "Hear The Bells", ArtistId = 17, AlbumId = 21, DurationSeconds = 285, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/1/6/0/d16afde306ca98c9e1e901857d4d0b09.mp3?hdnea=exp=1776617701~acl=/api/1/1/d/1/6/0/d16afde306ca98c9e1e901857d4d0b09.mp3*~data=user_id=0,application_id=42~hmac=8abf570035cbf193cbf95a2599ed0a5b6f40d86564194afe2477bee88709d38f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 310, ExternalTrackId = "81836832", Source = "Deezer", Title = "Natural Light", ArtistId = 17, AlbumId = 21, DurationSeconds = 141, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/e/8/0/ae8b397bf3172fc457f5382b73c5a86c.mp3?hdnea=exp=1776617702~acl=/api/1/1/a/e/8/0/ae8b397bf3172fc457f5382b73c5a86c.mp3*~data=user_id=0,application_id=42~hmac=e3d438f1d67f790683eb36ac6c768321f480bdca5a1ab846fbd95f94f8d58d96", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf61044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 311, ExternalTrackId = "81836834", Source = "Deezer", Title = "Unfold", ArtistId = 17, AlbumId = 21, DurationSeconds = 264, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/6/f/0/e6fef39c7e80bbe03188f8e0ba882e79.mp3?hdnea=exp=1776617702~acl=/api/1/1/e/6/f/0/e6fef39c7e80bbe03188f8e0ba882e79.mp3*~data=user_id=0,application_id=42~hmac=3b118b9dfca2d18ede921609fe68fa6673cecca3e1833b3bfaf14d47180eb415", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf61044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 312, ExternalTrackId = "81836836", Source = "Deezer", Title = "Sea Of Voices", ArtistId = 17, AlbumId = 21, DurationSeconds = 298, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/d/e/0/8de871a091580fae53369e0bfca9f178.mp3?hdnea=exp=1776617703~acl=/api/1/1/8/d/e/0/8de871a091580fae53369e0bfca9f178.mp3*~data=user_id=0,application_id=42~hmac=326ea0e1da77e33b0b3b1be5bb5d33f84df1e22d2dd8028948f7af4a56625c76", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf61044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 313, ExternalTrackId = "81836838", Source = "Deezer", Title = "Fellow Feeling", ArtistId = 17, AlbumId = 21, DurationSeconds = 349, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/8/c/0/08c6dc60516e149deb5be3436815f580.mp3?hdnea=exp=1776617703~acl=/api/1/1/0/8/c/0/08c6dc60516e149deb5be3436815f580.mp3*~data=user_id=0,application_id=42~hmac=72da764c4df29b0df8520afabe4a25b56ac6fe6a8b9eea96695f160ec8592538", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf61044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 314, ExternalTrackId = "81836840", Source = "Deezer", Title = "Goodbye To A World", ArtistId = 17, AlbumId = 21, DurationSeconds = 328, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/c/e/0/4ce74b9c9b28339377af8dec95fce8fb.mp3?hdnea=exp=1776617704~acl=/api/1/1/4/c/e/0/4ce74b9c9b28339377af8dec95fce8fb.mp3*~data=user_id=0,application_id=42~hmac=b51d1cb0810cdacd95b808c804a183ff14fc4d15ca5417b7810d9886bbd6ad04", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf61044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 315, ExternalTrackId = "830336912", Source = "Deezer", Title = "Golden", ArtistId = 18, AlbumId = 22, DurationSeconds = 208, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/1/5/0/f15347aeb5149d0ba123107d1ed7a566.mp3?hdnea=exp=1776617705~acl=/api/1/1/f/1/5/0/f15347aeb5149d0ba123107d1ed7a566.mp3*~data=user_id=0,application_id=42~hmac=0dac288346d960f49e4a28b4766f307b0af7fbf954f222c3299387db04a69502", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 316, ExternalTrackId = "830336922", Source = "Deezer", Title = "Watermelon Sugar", ArtistId = 18, AlbumId = 22, DurationSeconds = 173, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/7/5/0/f75672f3225537bd26bb33f0f8947618.mp3?hdnea=exp=1776617705~acl=/api/1/1/f/7/5/0/f75672f3225537bd26bb33f0f8947618.mp3*~data=user_id=0,application_id=42~hmac=5e1b5ebcfb370d76e4907a024009f540227c50566f58d2b5449842370e35cf7e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 317, ExternalTrackId = "830336932", Source = "Deezer", Title = "Adore You", ArtistId = 18, AlbumId = 22, DurationSeconds = 207, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/9/0/0/390b7601c8e7ca401635ee680d222c72.mp3?hdnea=exp=1776617706~acl=/api/1/1/3/9/0/0/390b7601c8e7ca401635ee680d222c72.mp3*~data=user_id=0,application_id=42~hmac=c98c427fefb27a7b6fa9c1bfc7f848069615cd6517e9f7483a924fbbdcd2829a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 318, ExternalTrackId = "830336942", Source = "Deezer", Title = "Lights Up", ArtistId = 18, AlbumId = 22, DurationSeconds = 174, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/3/f/0/f3f429f46aff48643f59c093970f6be1.mp3?hdnea=exp=1776617706~acl=/api/1/1/f/3/f/0/f3f429f46aff48643f59c093970f6be1.mp3*~data=user_id=0,application_id=42~hmac=ad1c3e033a638d75b72513062c070ca0d7749efb99bb52a86ce4fa1dba5ae7ba", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 319, ExternalTrackId = "830336952", Source = "Deezer", Title = "Cherry", ArtistId = 18, AlbumId = 22, DurationSeconds = 259, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/4/7/0/447a9a50779cda7ef06aa6c888b2409a.mp3?hdnea=exp=1776617707~acl=/api/1/1/4/4/7/0/447a9a50779cda7ef06aa6c888b2409a.mp3*~data=user_id=0,application_id=42~hmac=5698d25389f0265d283bcf9478375aa8473caeb5d04cd21b09e215e28554648f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 320, ExternalTrackId = "830336962", Source = "Deezer", Title = "Falling", ArtistId = 18, AlbumId = 22, DurationSeconds = 240, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/0/0/0/8002da25d31e698ddabb1049c1604d2a.mp3?hdnea=exp=1776617707~acl=/api/1/1/8/0/0/0/8002da25d31e698ddabb1049c1604d2a.mp3*~data=user_id=0,application_id=42~hmac=c63bc8ed40956ad99cb0b295930d56ef5895577b4c74f8fa8bf65385782e0046", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 321, ExternalTrackId = "830336972", Source = "Deezer", Title = "To Be So Lonely", ArtistId = 18, AlbumId = 22, DurationSeconds = 192, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/8/1/0/381d73652dfa2292c8336b442c2ae511.mp3?hdnea=exp=1776617708~acl=/api/1/1/3/8/1/0/381d73652dfa2292c8336b442c2ae511.mp3*~data=user_id=0,application_id=42~hmac=ba500b42f0f613644ca6575e3c3603fceb205aa71b121015b7247bf86d7edd69", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 322, ExternalTrackId = "830336982", Source = "Deezer", Title = "She", ArtistId = 18, AlbumId = 22, DurationSeconds = 362, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/5/7/0/05742ce31f3fcee023f4caede3beead7.mp3?hdnea=exp=1776617709~acl=/api/1/1/0/5/7/0/05742ce31f3fcee023f4caede3beead7.mp3*~data=user_id=0,application_id=42~hmac=6751cbca916d258eece0d119e6cd2858d3871f6046819138cde40f958d6e7554", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 323, ExternalTrackId = "830336992", Source = "Deezer", Title = "Sunflower, Vol. 6", ArtistId = 18, AlbumId = 22, DurationSeconds = 221, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/f/0/0/8f09e08267faccb2ee40a2d1d9999fa2.mp3?hdnea=exp=1776617709~acl=/api/1/1/8/f/0/0/8f09e08267faccb2ee40a2d1d9999fa2.mp3*~data=user_id=0,application_id=42~hmac=e044f2be7a7efe8063bbf5a02717ae43bc4f3f7a4d7c17b6bae9de516bd3c43c", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 324, ExternalTrackId = "830337002", Source = "Deezer", Title = "Canyon Moon", ArtistId = 18, AlbumId = 22, DurationSeconds = 189, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/2/8/0/9280ce9cf08dcb672e57234e1051a2bc.mp3?hdnea=exp=1776617710~acl=/api/1/1/9/2/8/0/9280ce9cf08dcb672e57234e1051a2bc.mp3*~data=user_id=0,application_id=42~hmac=07f5562dd039b942c1da75d4264107826216d3b05d9078b414043676b3b5ba45", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 325, ExternalTrackId = "830337012", Source = "Deezer", Title = "Treat People With Kindness", ArtistId = 18, AlbumId = 22, DurationSeconds = 197, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/a/e/0/3ae16ba2edb32b85f010a8e77b8ae050.mp3?hdnea=exp=1776617710~acl=/api/1/1/3/a/e/0/3ae16ba2edb32b85f010a8e77b8ae050.mp3*~data=user_id=0,application_id=42~hmac=0f181f931b0e59994a62110b47a807dc9286c718d0cdf7365ef32c5a3a7c3214", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 326, ExternalTrackId = "830337022", Source = "Deezer", Title = "Fine Line", ArtistId = 18, AlbumId = 22, DurationSeconds = 377, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/2/2/0/b22a10592fbb46a1789da222c2899518.mp3?hdnea=exp=1776617711~acl=/api/1/1/b/2/2/0/b22a10592fbb46a1789da222c2899518.mp3*~data=user_id=0,application_id=42~hmac=2f5dd7bc522ca981ae7e65811463c4d50bc5edf7d08d470f23f005e7ac42edf0", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 327, ExternalTrackId = "4677472", Source = "Deezer", Title = "Curtains Up", ArtistId = 19, AlbumId = 23, DurationSeconds = 47, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/c/0/0/8c05098848d90a3d461fc3f1cd1fe286.mp3?hdnea=exp=1776617712~acl=/api/1/1/8/c/0/0/8c05098848d90a3d461fc3f1cd1fe286.mp3*~data=user_id=0,application_id=42~hmac=2816f8adb23595e54299413e5e375c06a248db918d64c056950548e3adfc0442", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 328, ExternalTrackId = "4677473", Source = "Deezer", Title = "Evil Deeds", ArtistId = 19, AlbumId = 23, DurationSeconds = 260, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/c/f/0/8cfcdf18336a5453f0222729b7a8cf9d.mp3?hdnea=exp=1776617712~acl=/api/1/1/8/c/f/0/8cfcdf18336a5453f0222729b7a8cf9d.mp3*~data=user_id=0,application_id=42~hmac=03db0cd96e61996d9164729519f7e1e68f3aede89cbb117af053b15b3b3aac4b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 329, ExternalTrackId = "4677474", Source = "Deezer", Title = "Never Enough", ArtistId = 19, AlbumId = 23, DurationSeconds = 160, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/f/f/0/eff5f8a60235c333aac072046e143efd.mp3?hdnea=exp=1776617713~acl=/api/1/1/e/f/f/0/eff5f8a60235c333aac072046e143efd.mp3*~data=user_id=0,application_id=42~hmac=ab8f13da8ea1db6e26f225cd5f3e895ef19c9bf900f519919dae1fee256a3fe2", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 330, ExternalTrackId = "4677475", Source = "Deezer", Title = "Yellow Brick Road", ArtistId = 19, AlbumId = 23, DurationSeconds = 346, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/f/f/0/5ff4bd325de42a5764671ce80950d298.mp3?hdnea=exp=1776617714~acl=/api/1/1/5/f/f/0/5ff4bd325de42a5764671ce80950d298.mp3*~data=user_id=0,application_id=42~hmac=876d12371ae735c1177d358ea226ae179ca2228f0272d3390d741ca23f137ba6", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 331, ExternalTrackId = "4677476", Source = "Deezer", Title = "Like Toy Soldiers", ArtistId = 19, AlbumId = 23, DurationSeconds = 297, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/2/c/0/52c5c0926b536bed40bb9d4e7b16c69c.mp3?hdnea=exp=1776617714~acl=/api/1/1/5/2/c/0/52c5c0926b536bed40bb9d4e7b16c69c.mp3*~data=user_id=0,application_id=42~hmac=ac7e42e189ac927fea15931be30bcc0848b8d3eaa88a58a04f51b6fc678a2396", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 332, ExternalTrackId = "4677477", Source = "Deezer", Title = "Mosh", ArtistId = 19, AlbumId = 23, DurationSeconds = 318, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/3/0/0/1304838c460d5387de92a94c0873eb34.mp3?hdnea=exp=1776617715~acl=/api/1/1/1/3/0/0/1304838c460d5387de92a94c0873eb34.mp3*~data=user_id=0,application_id=42~hmac=607d7ba79593a5baeb1c7176af2398b211648e9d8a385cfc0eb0e8933ecc220c", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 333, ExternalTrackId = "4677478", Source = "Deezer", Title = "Puke", ArtistId = 19, AlbumId = 23, DurationSeconds = 248, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/6/5/0/b653d71127fc81f9be56d6dcc3e949d5.mp3?hdnea=exp=1776617716~acl=/api/1/1/b/6/5/0/b653d71127fc81f9be56d6dcc3e949d5.mp3*~data=user_id=0,application_id=42~hmac=1ec81b96f422b58fce47513c71d54462af3fff89cf126a0f66b2f11d6d94c8a6", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 334, ExternalTrackId = "4677479", Source = "Deezer", Title = "My 1st Single", ArtistId = 19, AlbumId = 23, DurationSeconds = 303, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/6/5/0/965dedc12b5ee152a7b86e8b251c5771.mp3?hdnea=exp=1776617716~acl=/api/1/1/9/6/5/0/965dedc12b5ee152a7b86e8b251c5771.mp3*~data=user_id=0,application_id=42~hmac=d2351f0cb9a8f6fdbc85114341d1f5fcd1a28a8db039e20e58b2c6d3cd71621a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 335, ExternalTrackId = "4677480", Source = "Deezer", Title = "Paul (Skit)", ArtistId = 19, AlbumId = 23, DurationSeconds = 32, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/8/5/0/5851490f7cd03ea33b26a8ed55fc5f31.mp3?hdnea=exp=1776617717~acl=/api/1/1/5/8/5/0/5851490f7cd03ea33b26a8ed55fc5f31.mp3*~data=user_id=0,application_id=42~hmac=cb5b1b5fcbb5c99b4485fcdf6323780554f8f9e23e0209948d83e4d97166547c", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 336, ExternalTrackId = "4677481", Source = "Deezer", Title = "Rain Man", ArtistId = 19, AlbumId = 23, DurationSeconds = 314, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/d/8/0/3d8fdcb0c06093e0976e62d6278b8298.mp3?hdnea=exp=1776617717~acl=/api/1/1/3/d/8/0/3d8fdcb0c06093e0976e62d6278b8298.mp3*~data=user_id=0,application_id=42~hmac=ee854ecfebc49945b66fe05f4e702feb5579bc1e3347bc57cc96786dc1def746", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 337, ExternalTrackId = "4677482", Source = "Deezer", Title = "Big Weenie", ArtistId = 19, AlbumId = 23, DurationSeconds = 267, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/b/e/0/abe770ecf43c87dbb31e90ab005cea57.mp3?hdnea=exp=1776617718~acl=/api/1/1/a/b/e/0/abe770ecf43c87dbb31e90ab005cea57.mp3*~data=user_id=0,application_id=42~hmac=e1f029a235a1883e4b64a535da7f11f7e386965bb5be6cb183d38b1fec92c25d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 338, ExternalTrackId = "4677483", Source = "Deezer", Title = "Em Calls Paul (Skit)", ArtistId = 19, AlbumId = 23, DurationSeconds = 72, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/b/8/0/2b8e973fc9fd05a7ae834663810ff442.mp3?hdnea=exp=1776617719~acl=/api/1/1/2/b/8/0/2b8e973fc9fd05a7ae834663810ff442.mp3*~data=user_id=0,application_id=42~hmac=ac9a03e72e25324c16b415decb313b38927eaf27a4ea39eb6ea1522e773a60ee", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 339, ExternalTrackId = "4677484", Source = "Deezer", Title = "Just Lose It", ArtistId = 19, AlbumId = 23, DurationSeconds = 249, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/1/2/0/d12635eb6d8b1bd190b2012114c2b0f6.mp3?hdnea=exp=1776617719~acl=/api/1/1/d/1/2/0/d12635eb6d8b1bd190b2012114c2b0f6.mp3*~data=user_id=0,application_id=42~hmac=aaa95e974cdac7b2db9db247876d94cc1210dac89b995e22cb170c54fc8b6f76", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 340, ExternalTrackId = "4677485", Source = "Deezer", Title = "Ass Like That", ArtistId = 19, AlbumId = 23, DurationSeconds = 265, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/a/f/0/4afdabe55907b28abf1cdd0a924f0fb7.mp3?hdnea=exp=1776617720~acl=/api/1/1/4/a/f/0/4afdabe55907b28abf1cdd0a924f0fb7.mp3*~data=user_id=0,application_id=42~hmac=7fc0bf261f08ca7b22c53c2c32b2f0fdb949f7998f9b9be0400a1a666999f6c8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 341, ExternalTrackId = "4677486", Source = "Deezer", Title = "Spend Some Time", ArtistId = 19, AlbumId = 23, DurationSeconds = 310, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/5/a/0/75a36ed39532b3cdf56f2c01ada1dbb0.mp3?hdnea=exp=1776617721~acl=/api/1/1/7/5/a/0/75a36ed39532b3cdf56f2c01ada1dbb0.mp3*~data=user_id=0,application_id=42~hmac=d35b7f8a4d284ddbd75ea70bf54287373368c82ef25a62aea1abd02494333873", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 342, ExternalTrackId = "4677487", Source = "Deezer", Title = "Mockingbird", ArtistId = 19, AlbumId = 23, DurationSeconds = 251, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/1/6/0/a16a1bc81d5f224c48f61d1372aa3f3a.mp3?hdnea=exp=1776617721~acl=/api/1/1/a/1/6/0/a16a1bc81d5f224c48f61d1372aa3f3a.mp3*~data=user_id=0,application_id=42~hmac=56b5720c456b41a5ee3e4182710a0f6c5ff7e61a3ac15e51d0cff2babbc67775", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 343, ExternalTrackId = "4677488", Source = "Deezer", Title = "Crazy In Love", ArtistId = 19, AlbumId = 23, DurationSeconds = 242, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/4/4/0/7440cc846e82401560be09a9943f2247.mp3?hdnea=exp=1776617722~acl=/api/1/1/7/4/4/0/7440cc846e82401560be09a9943f2247.mp3*~data=user_id=0,application_id=42~hmac=3a03bdfc9efb71630bd96d673473c874b046f1b0402125db71c504524d6a8792", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 344, ExternalTrackId = "4677489", Source = "Deezer", Title = "One Shot 2 Shot", ArtistId = 19, AlbumId = 23, DurationSeconds = 267, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/f/8/0/df8871faa945367781bce6c0d5cae663.mp3?hdnea=exp=1776617723~acl=/api/1/1/d/f/8/0/df8871faa945367781bce6c0d5cae663.mp3*~data=user_id=0,application_id=42~hmac=19b2b69208fe7cc8c675a880c8e228a43a11e56c5299968cfd828ebe98bdfef7", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 345, ExternalTrackId = "4677490", Source = "Deezer", Title = "Final Thought (Skit)", ArtistId = 19, AlbumId = 23, DurationSeconds = 30, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/4/a/0/e4ae674c1cdefc5ff533a4565e013b82.mp3?hdnea=exp=1776617723~acl=/api/1/1/e/4/a/0/e4ae674c1cdefc5ff533a4565e013b82.mp3*~data=user_id=0,application_id=42~hmac=abbbf91ec1337d4ded19851e27b0effe6e694f78098ea09d1ab88677da90d5aa", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 346, ExternalTrackId = "4677491", Source = "Deezer", Title = "Encore / Curtains Down", ArtistId = 19, AlbumId = 23, DurationSeconds = 347, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/3/7/0/737102da98bd48b7ba3dd998d786fad6.mp3?hdnea=exp=1776617724~acl=/api/1/1/7/3/7/0/737102da98bd48b7ba3dd998d786fad6.mp3*~data=user_id=0,application_id=42~hmac=14e6fdc0250bead478d353447ebff5bcbef77a96e6aba3bbf7a517ad1fb2ec2a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 347, ExternalTrackId = "4677492", Source = "Deezer", Title = "We As Americans (Album Version)", ArtistId = 19, AlbumId = 23, DurationSeconds = 276, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/d/b/0/3db0ef75074e8e42d3217cf46b8375b4.mp3?hdnea=exp=1776617724~acl=/api/1/1/3/d/b/0/3db0ef75074e8e42d3217cf46b8375b4.mp3*~data=user_id=0,application_id=42~hmac=a1f0a39f5e212edfcc73f7ffa92ecf2506697d25ee9d8fc963650194705e351f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 348, ExternalTrackId = "4677493", Source = "Deezer", Title = "Love You More (Album Version)", ArtistId = 19, AlbumId = 23, DurationSeconds = 284, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/2/d/0/72dd494b8e592aae1190555c9cc89c7a.mp3?hdnea=exp=1776617725~acl=/api/1/1/7/2/d/0/72dd494b8e592aae1190555c9cc89c7a.mp3*~data=user_id=0,application_id=42~hmac=419aa8c6f5559771f8686cb65aa02d6377f584ebea632e1269cea22ea5b52639", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 349, ExternalTrackId = "4677494", Source = "Deezer", Title = "Ricky Ticky Toc (Album Version)", ArtistId = 19, AlbumId = 23, DurationSeconds = 172, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/7/e/0/97e8f13eaf8424303ba762dfcd918eaf.mp3?hdnea=exp=1776617726~acl=/api/1/1/9/7/e/0/97e8f13eaf8424303ba762dfcd918eaf.mp3*~data=user_id=0,application_id=42~hmac=c60f232087540e11987e912b38d6ab2ec3953fb367b2b93d5ba067ca51770070", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 350, ExternalTrackId = "15593559", Source = "Deezer", Title = "Jam", ArtistId = 11, AlbumId = 24, DurationSeconds = 339, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/5/7/0/1574ba54845cba4a94e309e9cbdb7185.mp3?hdnea=exp=1776617726~acl=/api/1/1/1/5/7/0/1574ba54845cba4a94e309e9cbdb7185.mp3*~data=user_id=0,application_id=42~hmac=eb47b11dce0b1d92be560b62388f0a25e0a90c53caea6b605da0b5202e7efee4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 351, ExternalTrackId = "15593560", Source = "Deezer", Title = "Why You Wanna Trip on Me", ArtistId = 11, AlbumId = 24, DurationSeconds = 325, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/0/b/0/50bdb7e073d6a65689fa07553a32fa3c.mp3?hdnea=exp=1776617727~acl=/api/1/1/5/0/b/0/50bdb7e073d6a65689fa07553a32fa3c.mp3*~data=user_id=0,application_id=42~hmac=04af12a68707d45a1a78d4c74acfc31b26ba8af2753d66bcaf9dadd42230e809", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 352, ExternalTrackId = "15593561", Source = "Deezer", Title = "In the Closet", ArtistId = 11, AlbumId = 24, DurationSeconds = 392, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/6/e/0/06e61c06fa8f17bacaa015506006dc89.mp3?hdnea=exp=1776617728~acl=/api/1/1/0/6/e/0/06e61c06fa8f17bacaa015506006dc89.mp3*~data=user_id=0,application_id=42~hmac=686091a410734570affbb6485b2a0aea35def37da67b6ebabf4d11706cc17bd3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 353, ExternalTrackId = "15593562", Source = "Deezer", Title = "She Drives Me Wild", ArtistId = 11, AlbumId = 24, DurationSeconds = 221, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/b/b/0/ebb5d0f438e8059a1a190bb57cc7ccc9.mp3?hdnea=exp=1776617728~acl=/api/1/1/e/b/b/0/ebb5d0f438e8059a1a190bb57cc7ccc9.mp3*~data=user_id=0,application_id=42~hmac=5ae272c07717abc23d09d439b64a641f1ef13e9c77266cf79376af5c2afd8750", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 354, ExternalTrackId = "15593563", Source = "Deezer", Title = "Remember the Time", ArtistId = 11, AlbumId = 24, DurationSeconds = 239, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/a/a/0/3aa8e58e024bd295afd0ba22a1a0c15a.mp3?hdnea=exp=1776617729~acl=/api/1/1/3/a/a/0/3aa8e58e024bd295afd0ba22a1a0c15a.mp3*~data=user_id=0,application_id=42~hmac=5c66498e5fde2fe05e052d8b6579410d4e10d6b77c95b52c569d1a2a081ad8f4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 355, ExternalTrackId = "15593564", Source = "Deezer", Title = "Can't Let Her Get Away", ArtistId = 11, AlbumId = 24, DurationSeconds = 299, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/b/7/0/fb77c076d384fa4c6cc080e4456f1fb8.mp3?hdnea=exp=1776617730~acl=/api/1/1/f/b/7/0/fb77c076d384fa4c6cc080e4456f1fb8.mp3*~data=user_id=0,application_id=42~hmac=2ea55efd0d77e620681bae8cfddef8118e32c16cd3ee71cb7e11b9f942d6e0d5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 356, ExternalTrackId = "15593565", Source = "Deezer", Title = "Heal the World", ArtistId = 11, AlbumId = 24, DurationSeconds = 384, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/b/9/0/db962c00b49fc4ed70586c427a35a808.mp3?hdnea=exp=1776617730~acl=/api/1/1/d/b/9/0/db962c00b49fc4ed70586c427a35a808.mp3*~data=user_id=0,application_id=42~hmac=59a4fb4896daad89b118afab0e0d6a66694ca93868cdbe8ccb1be425ff489a3d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 357, ExternalTrackId = "15593566", Source = "Deezer", Title = "Black or White", ArtistId = 11, AlbumId = 24, DurationSeconds = 256, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/5/2/0/2523fceaa336fa3aedcd33a8a4e2bda9.mp3?hdnea=exp=1776617731~acl=/api/1/1/2/5/2/0/2523fceaa336fa3aedcd33a8a4e2bda9.mp3*~data=user_id=0,application_id=42~hmac=6057d0d25e58c8a2b73df72c55adb7cc48b044a04a427bdc6978248dae1ec858", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 358, ExternalTrackId = "15593567", Source = "Deezer", Title = "Who Is It", ArtistId = 11, AlbumId = 24, DurationSeconds = 395, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/6/a/0/e6a5e2bd9cfa0088e6ffff9130799d0d.mp3?hdnea=exp=1776617732~acl=/api/1/1/e/6/a/0/e6a5e2bd9cfa0088e6ffff9130799d0d.mp3*~data=user_id=0,application_id=42~hmac=d4f1b8377b7b6c3e8cf20716d97f71d7b174486f559a255b13057e2077930e9f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 359, ExternalTrackId = "15593568", Source = "Deezer", Title = "Give In to Me", ArtistId = 11, AlbumId = 24, DurationSeconds = 330, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/d/b/0/6db2cac551f48f001c6c15b4da5c7a91.mp3?hdnea=exp=1776617732~acl=/api/1/1/6/d/b/0/6db2cac551f48f001c6c15b4da5c7a91.mp3*~data=user_id=0,application_id=42~hmac=144d16d23cff95971be33795e4edba6567008fd73ef36ee81c57a270bcc3d006", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 360, ExternalTrackId = "15593569", Source = "Deezer", Title = "Will You Be There", ArtistId = 11, AlbumId = 24, DurationSeconds = 460, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/c/0/0/dc04050bea0490b7018c7ec01239d187.mp3?hdnea=exp=1776617733~acl=/api/1/1/d/c/0/0/dc04050bea0490b7018c7ec01239d187.mp3*~data=user_id=0,application_id=42~hmac=4f68a34bdd0a1f2beb9af62416dd02923dc89352a754f4fc106a15f66483e097", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 361, ExternalTrackId = "15593570", Source = "Deezer", Title = "Keep the Faith", ArtistId = 11, AlbumId = 24, DurationSeconds = 357, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/9/9/0/c993f4a291d6a5d0cda02ca470e7d02a.mp3?hdnea=exp=1776617733~acl=/api/1/1/c/9/9/0/c993f4a291d6a5d0cda02ca470e7d02a.mp3*~data=user_id=0,application_id=42~hmac=d0a0bd2a5228c3d9171e496c24ad2fec3a73350a8bafa90a77b40199329f8c31", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 362, ExternalTrackId = "15593571", Source = "Deezer", Title = "Gone Too Soon", ArtistId = 11, AlbumId = 24, DurationSeconds = 202, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/4/7/0/147f93477cdb2fbdeacac058a932d315.mp3?hdnea=exp=1776617734~acl=/api/1/1/1/4/7/0/147f93477cdb2fbdeacac058a932d315.mp3*~data=user_id=0,application_id=42~hmac=1f54c962f6010d2ee93e299db67e14d9b13cba669f01c817ff6ad13d3af58ffd", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 363, ExternalTrackId = "15593572", Source = "Deezer", Title = "Dangerous", ArtistId = 11, AlbumId = 24, DurationSeconds = 420, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/5/4/0/0544707e420323e791592dcafac73392.mp3?hdnea=exp=1776617735~acl=/api/1/1/0/5/4/0/0544707e420323e791592dcafac73392.mp3*~data=user_id=0,application_id=42~hmac=53a6ae465f07aa4897c49f68251042bad229422d724c2acb154019a7ae1c5fd3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 364, ExternalTrackId = "8011849", Source = "Deezer", Title = "Grenade", ArtistId = 20, AlbumId = 25, DurationSeconds = 222, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/2/c/0/12c6a1605606deacfd9f0d8fde07abf2.mp3?hdnea=exp=1776617735~acl=/api/1/1/1/2/c/0/12c6a1605606deacfd9f0d8fde07abf2.mp3*~data=user_id=0,application_id=42~hmac=c53e53d0d901cdfc9f406bd58cc3586951cda979e6726c6e547fff6f1235d1f4", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 365, ExternalTrackId = "8011850", Source = "Deezer", Title = "Just the Way You Are", ArtistId = 20, AlbumId = 25, DurationSeconds = 220, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/9/f/0/f9fe7ab157c5496285f09fbefd7bc647.mp3?hdnea=exp=1776617736~acl=/api/1/1/f/9/f/0/f9fe7ab157c5496285f09fbefd7bc647.mp3*~data=user_id=0,application_id=42~hmac=b5745da07081780f8005b03be134c0fd58e7af9eec718f173a4ee48f03e30516", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 366, ExternalTrackId = "8011851", Source = "Deezer", Title = "Our First Time", ArtistId = 20, AlbumId = 25, DurationSeconds = 243, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/7/3/0/273f1c64e2dccae7f6d683687d5583d8.mp3?hdnea=exp=1776617737~acl=/api/1/1/2/7/3/0/273f1c64e2dccae7f6d683687d5583d8.mp3*~data=user_id=0,application_id=42~hmac=2fc53c3f868720216effbd111bff6dfc38e7caab79d48278ba5e2b81d6ca0755", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 367, ExternalTrackId = "8011852", Source = "Deezer", Title = "Runaway Baby", ArtistId = 20, AlbumId = 25, DurationSeconds = 148, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/8/b/0/e8b20b9a893aff0a596057e9779e0978.mp3?hdnea=exp=1776617737~acl=/api/1/1/e/8/b/0/e8b20b9a893aff0a596057e9779e0978.mp3*~data=user_id=0,application_id=42~hmac=8b57a72536092ca1b324b85638e562d303cbb90f156801f5af1c4e998e021801", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 368, ExternalTrackId = "8011853", Source = "Deezer", Title = "The Lazy Song", ArtistId = 20, AlbumId = 25, DurationSeconds = 189, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/6/6/0/e66991b4b9756d46d54e556f6d04be89.mp3?hdnea=exp=1776617738~acl=/api/1/1/e/6/6/0/e66991b4b9756d46d54e556f6d04be89.mp3*~data=user_id=0,application_id=42~hmac=a25a9cf3ff5dffd6e12a5a50718cf2cac474ea6f03dba8fa88d4d049bffc17ad", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 369, ExternalTrackId = "8011854", Source = "Deezer", Title = "Marry You", ArtistId = 20, AlbumId = 25, DurationSeconds = 230, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/6/8/0/b688162cb4f71ff517d7ab018edea6ff.mp3?hdnea=exp=1776617739~acl=/api/1/1/b/6/8/0/b688162cb4f71ff517d7ab018edea6ff.mp3*~data=user_id=0,application_id=42~hmac=bbb79d161b1e2024a1fef99331e3582d7ebd9f70674c8edfb1c89468d495cb0e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 370, ExternalTrackId = "8011855", Source = "Deezer", Title = "Talking to the Moon", ArtistId = 20, AlbumId = 25, DurationSeconds = 217, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/a/0/0/ba022293535d46e186a8abfbee83cf4e.mp3?hdnea=exp=1776617739~acl=/api/1/1/b/a/0/0/ba022293535d46e186a8abfbee83cf4e.mp3*~data=user_id=0,application_id=42~hmac=69a9e3a96326bbb5abecabf0438682516d0a70dea2e465b2221a251d2e22213c", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 371, ExternalTrackId = "8011856", Source = "Deezer", Title = "Liquor Store Blues (feat. Damian Marley)", ArtistId = 20, AlbumId = 25, DurationSeconds = 229, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/a/0/0/9a008dea441c6bbda296c31b8b8cb093.mp3?hdnea=exp=1776617740~acl=/api/1/1/9/a/0/0/9a008dea441c6bbda296c31b8b8cb093.mp3*~data=user_id=0,application_id=42~hmac=98ef8d1e3f3960fc245b9b393b6288fe13d33225c082f43d7d7bcaa2998ce3f9", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 372, ExternalTrackId = "8011857", Source = "Deezer", Title = "Count on Me", ArtistId = 20, AlbumId = 25, DurationSeconds = 197, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/5/9/0/c59c42bf485eefa3b4b3e83d4e1e966e.mp3?hdnea=exp=1776617740~acl=/api/1/1/c/5/9/0/c59c42bf485eefa3b4b3e83d4e1e966e.mp3*~data=user_id=0,application_id=42~hmac=7b809b575fa3dea4f5b1df171e2cdc191824a13dbd8c5319bcf37cd767d774d8", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 373, ExternalTrackId = "8011858", Source = "Deezer", Title = "The Other Side (feat. CeeLo Green and B.o.B)", ArtistId = 20, AlbumId = 25, DurationSeconds = 228, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/e/9/0/3e93bcfb16740cac5dfe2bb95b124279.mp3?hdnea=exp=1776617741~acl=/api/1/1/3/e/9/0/3e93bcfb16740cac5dfe2bb95b124279.mp3*~data=user_id=0,application_id=42~hmac=4d245a2398cea78fe4db115ea0c52af38d60c3e46f328ae5ef583beef1eb7de2", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 374, ExternalTrackId = "8011859", Source = "Deezer", Title = "Somewhere in Brooklyn", ArtistId = 20, AlbumId = 25, DurationSeconds = 181, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/d/7/0/9d7a47869ab7ddb7c5c3a898d02010bc.mp3?hdnea=exp=1776617742~acl=/api/1/1/9/d/7/0/9d7a47869ab7ddb7c5c3a898d02010bc.mp3*~data=user_id=0,application_id=42~hmac=d41cfeb66a4aa9405d62da57b6ca93e078168160d064c63881e1565a95e7430a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 375, ExternalTrackId = "8011860", Source = "Deezer", Title = "Talking to the Moon (Acoustic Piano Version)", ArtistId = 20, AlbumId = 25, DurationSeconds = 217, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/7/3/0/373dd5e4f71739480bdec62e2918d405.mp3?hdnea=exp=1776617742~acl=/api/1/1/3/7/3/0/373dd5e4f71739480bdec62e2918d405.mp3*~data=user_id=0,application_id=42~hmac=0dd26ae811f7493d5657dca588e90d0d579dd7853751c097b1048df1e4e5d9c9", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 376, ExternalTrackId = "96816466", Source = "Deezer", Title = "Isometric (Intro)", ArtistId = 21, AlbumId = 26, DurationSeconds = 80, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/f/0/0/ef00cf34b7db6e717d7ee3af3aacc417.mp3?hdnea=exp=1776617743~acl=/api/1/1/e/f/0/0/ef00cf34b7db6e717d7ee3af3aacc417.mp3*~data=user_id=0,application_id=42~hmac=e61e85a019219640545a78fc28407c82b18008c5b64381a42a4bc9fee2336e5a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 377, ExternalTrackId = "96816468", Source = "Deezer", Title = "You're On (feat. Kyan)", ArtistId = 21, AlbumId = 26, DurationSeconds = 192, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/b/6/0/fb63165040ef64d276c534e975c7e438.mp3?hdnea=exp=1776617743~acl=/api/1/1/f/b/6/0/fb63165040ef64d276c534e975c7e438.mp3*~data=user_id=0,application_id=42~hmac=4b5fa84fe4cf5816ab08ec6c4ae25c542bdf2e8240a78be58b2e945a88508710", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 378, ExternalTrackId = "96816470", Source = "Deezer", Title = "OK", ArtistId = 21, AlbumId = 26, DurationSeconds = 182, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/b/2/0/bb225e3f78bb44fc952bdb17c24ace3a.mp3?hdnea=exp=1776617744~acl=/api/1/1/b/b/2/0/bb225e3f78bb44fc952bdb17c24ace3a.mp3*~data=user_id=0,application_id=42~hmac=e7c709e139de3939d303b614d676a4f933f602a6b5943cdd11da906b60811b87", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 379, ExternalTrackId = "96816472", Source = "Deezer", Title = "La Lune (feat. Dan Smith)", ArtistId = 21, AlbumId = 26, DurationSeconds = 219, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/9/5/0/b95b968e2e0e6ce3e4b3cc7b5e706110.mp3?hdnea=exp=1776617745~acl=/api/1/1/b/9/5/0/b95b968e2e0e6ce3e4b3cc7b5e706110.mp3*~data=user_id=0,application_id=42~hmac=90d9138655bc3fc831241c16985cb06fc4d535abe78334ab2297da7f97d6aa28", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 380, ExternalTrackId = "96816474", Source = "Deezer", Title = "Pay No Mind (feat. Passion Pit)", ArtistId = 21, AlbumId = 26, DurationSeconds = 249, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/b/9/0/0/b9045052c40d582392dbbed0caba4cec.mp3?hdnea=exp=1776617745~acl=/api/1/1/b/9/0/0/b9045052c40d582392dbbed0caba4cec.mp3*~data=user_id=0,application_id=42~hmac=177e8ec199edf01700dee1e52bd8a0c2bc7b08d736d5e44cbed12582f30f8e49", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 381, ExternalTrackId = "96816476", Source = "Deezer", Title = "Beings", ArtistId = 21, AlbumId = 26, DurationSeconds = 215, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/4/9/0/7499593b2866a5361786e157add8e59c.mp3?hdnea=exp=1776617746~acl=/api/1/1/7/4/9/0/7499593b2866a5361786e157add8e59c.mp3*~data=user_id=0,application_id=42~hmac=487b522bec929bc1ac53940e128417844ca8791daa57dfd8562ca017a6d679f5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 382, ExternalTrackId = "96816478", Source = "Deezer", Title = "Imperium", ArtistId = 21, AlbumId = 26, DurationSeconds = 198, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/0/f/0/f0fe0ea98864f82dd6c1605ad08bdc1d.mp3?hdnea=exp=1776617747~acl=/api/1/1/f/0/f/0/f0fe0ea98864f82dd6c1605ad08bdc1d.mp3*~data=user_id=0,application_id=42~hmac=9c5dc4932537cc9481cb70ba4fb9908b78d202a5efe9ea2ee6f3dc0ae406d578", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 383, ExternalTrackId = "96816480", Source = "Deezer", Title = "Zephyr", ArtistId = 21, AlbumId = 26, DurationSeconds = 222, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/2/1/0/9214c957127dba85d39c338a40ebf372.mp3?hdnea=exp=1776617747~acl=/api/1/1/9/2/1/0/9214c957127dba85d39c338a40ebf372.mp3*~data=user_id=0,application_id=42~hmac=034b8c5cd0eac745ad0585fd9f886eaba3a693128c2faab70f2029c5fa12e275", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 384, ExternalTrackId = "96816482", Source = "Deezer", Title = "Nonsense (feat. Mark Foster)", ArtistId = 21, AlbumId = 26, DurationSeconds = 224, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/f/5/5/0/f55c79768a52221884b79b79d9e87371.mp3?hdnea=exp=1776617748~acl=/api/1/1/f/5/5/0/f55c79768a52221884b79b79d9e87371.mp3*~data=user_id=0,application_id=42~hmac=c65aebe085ac29e61c2b8b990a883d0886b63a2565c59bd75f791e47b87d0bb6", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 385, ExternalTrackId = "96816484", Source = "Deezer", Title = "Innocence (feat. Aquilo)", ArtistId = 21, AlbumId = 26, DurationSeconds = 224, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/f/7/0/3f79e3316676a7b791ddca88990eef7f.mp3?hdnea=exp=1776617749~acl=/api/1/1/3/f/7/0/3f79e3316676a7b791ddca88990eef7f.mp3*~data=user_id=0,application_id=42~hmac=65d0fdfcacfb8888ffea785e3785895c81af008bdbe0983e6861db3a33bfbf9c", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 386, ExternalTrackId = "96816486", Source = "Deezer", Title = "Pixel Empire", ArtistId = 21, AlbumId = 26, DurationSeconds = 244, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/b/e/0/5bec8d96daeb905b73d40814cfb9273a.mp3?hdnea=exp=1776617749~acl=/api/1/1/5/b/e/0/5bec8d96daeb905b73d40814cfb9273a.mp3*~data=user_id=0,application_id=42~hmac=8facf8a8820c1265811500fa4f8d32a608251b5d9491791b4d385ed68530e13a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 387, ExternalTrackId = "96816488", Source = "Deezer", Title = "Home", ArtistId = 21, AlbumId = 26, DurationSeconds = 225, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/a/c/0/dacfa1bb3edb8095a8f0f38db02b750d.mp3?hdnea=exp=1776617750~acl=/api/1/1/d/a/c/0/dacfa1bb3edb8095a8f0f38db02b750d.mp3*~data=user_id=0,application_id=42~hmac=15df26b5e57ee50f64fcca5acb49a9c77e6cb49f58706f7231202b459cd187ba", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

        new Song { Id = 388, ExternalTrackId = "96816490", Source = "Deezer", Title = "Icarus", ArtistId = 21, AlbumId = 26, DurationSeconds = 214, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/b/1/0/4b1d776be4952d206009ae95c977365f.mp3?hdnea=exp=1776617751~acl=/api/1/1/4/b/1/0/4b1d776be4952d206009ae95c977365f.mp3*~data=user_id=0,application_id=42~hmac=6d64e7a4d71eed735b4f9af2eecf8b8c7794154852fc7bfbb22158a6c37397b3", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 389, ExternalTrackId = "96816492", Source = "Deezer", Title = "Finale (feat. Nicholas Petricca)", ArtistId = 21, AlbumId = 26, DurationSeconds = 205, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/8/0/0/2807badb4c0a727411cb5d00880f24f6.mp3?hdnea=exp=1776617751~acl=/api/1/1/2/8/0/0/2807badb4c0a727411cb5d00880f24f6.mp3*~data=user_id=0,application_id=42~hmac=7e142ba84e9e5c2bba7412c8944fa3707de975df21e34834c18f2d70cb6ed1d2", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 390, ExternalTrackId = "96816494", Source = "Deezer", Title = "The City", ArtistId = 21, AlbumId = 26, DurationSeconds = 233, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/0/c/0/c0c8ae58f4eba7013d8d90c7842c7132.mp3?hdnea=exp=1776617752~acl=/api/1/1/c/0/c/0/c0c8ae58f4eba7013d8d90c7842c7132.mp3*~data=user_id=0,application_id=42~hmac=72b4981a212d0f075b1d406c8d3c76d2fdc08d298f93511df77485386e34bc7c", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 391, ExternalTrackId = "96816496", Source = "Deezer", Title = "Cut the Kid", ArtistId = 21, AlbumId = 26, DurationSeconds = 200, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/f/8/0/af87c5542ae097e0782bf917fc6c841c.mp3?hdnea=exp=1776617752~acl=/api/1/1/a/f/8/0/af87c5542ae097e0782bf917fc6c841c.mp3*~data=user_id=0,application_id=42~hmac=344ab7e77d8baf114106635ec1da5d240ade9a2e75430b8f9e54dd1d5b5d8140", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 392, ExternalTrackId = "96816498", Source = "Deezer", Title = "Technicolor", ArtistId = 21, AlbumId = 26, DurationSeconds = 385, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/1/5/0/0157cb83b09466e5f53bff0e7add7c6f.mp3?hdnea=exp=1776617753~acl=/api/1/1/0/1/5/0/0157cb83b09466e5f53bff0e7add7c6f.mp3*~data=user_id=0,application_id=42~hmac=6adf31e4c04946fe2f75cb08f301b9c20e0677fe29d546edac4eb0a4afa8e316", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 393, ExternalTrackId = "96816500", Source = "Deezer", Title = "Only Way Out (feat. Vancouver Sleep Clinic)", ArtistId = 21, AlbumId = 26, DurationSeconds = 226, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/d/7/0/ed7bb38e43528ffccb9fd2de02f64793.mp3?hdnea=exp=1776617754~acl=/api/1/1/e/d/7/0/ed7bb38e43528ffccb9fd2de02f64793.mp3*~data=user_id=0,application_id=42~hmac=fd53ff43be442598bb553619c3547c00fca883fa5a222aaeeeb8cf395d1b5633", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 394, ExternalTrackId = "1756569567", Source = "Deezer", Title = "Music For a Sushi Restaurant", ArtistId = 18, AlbumId = 27, DurationSeconds = 193, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/2/8/0/62805d25eef0e50839abd65fc4e7b24a.mp3?hdnea=exp=1776617754~acl=/api/1/1/6/2/8/0/62805d25eef0e50839abd65fc4e7b24a.mp3*~data=user_id=0,application_id=42~hmac=b05e2457ff7f6bfac4acc69b73f88947fedc3f90ba4a33f503b26f4d37786536", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 395, ExternalTrackId = "1756569577", Source = "Deezer", Title = "Late Night Talking", ArtistId = 18, AlbumId = 27, DurationSeconds = 177, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/e/2/b/0/e2b065b5086ac8ced8d6c10810dd130d.mp3?hdnea=exp=1776617755~acl=/api/1/1/e/2/b/0/e2b065b5086ac8ced8d6c10810dd130d.mp3*~data=user_id=0,application_id=42~hmac=57c4327e3e176da5c1b47e7f83e475560c5c2b5b5b09b49a5a25628901f3756d", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 396, ExternalTrackId = "1756569587", Source = "Deezer", Title = "Grapejuice", ArtistId = 18, AlbumId = 27, DurationSeconds = 191, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/4/3/0/643a9f5b524089677dbc626ffb7d3885.mp3?hdnea=exp=1776617756~acl=/api/1/1/6/4/3/0/643a9f5b524089677dbc626ffb7d3885.mp3*~data=user_id=0,application_id=42~hmac=f85b6411f7187b7327b75de891349b92b03694eaf88db36b1e662ab4756a1613", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 397, ExternalTrackId = "1756569597", Source = "Deezer", Title = "As It Was", ArtistId = 18, AlbumId = 27, DurationSeconds = 167, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/3/7/0/9372c78844baf74b7a06bd467151518a.mp3?hdnea=exp=1776617756~acl=/api/1/1/9/3/7/0/9372c78844baf74b7a06bd467151518a.mp3*~data=user_id=0,application_id=42~hmac=9ab91383a82eaad1a84b1dd0868a5454d3e0980b641b262c6261625dfbfdc08a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 398, ExternalTrackId = "1756569607", Source = "Deezer", Title = "Daylight", ArtistId = 18, AlbumId = 27, DurationSeconds = 164, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/8/3/0/083221212869bc97527931fcc2f475e5.mp3?hdnea=exp=1776617757~acl=/api/1/1/0/8/3/0/083221212869bc97527931fcc2f475e5.mp3*~data=user_id=0,application_id=42~hmac=1933c492945c5b46967c7bae0a88f1579b4b08f056486d38e5cf5c88af6f61ce", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 399, ExternalTrackId = "1756569617", Source = "Deezer", Title = "Little Freak", ArtistId = 18, AlbumId = 27, DurationSeconds = 202, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/5/9/0/d59e2ddf35781d4abe06e834d6b89b9e.mp3?hdnea=exp=1776617757~acl=/api/1/1/d/5/9/0/d59e2ddf35781d4abe06e834d6b89b9e.mp3*~data=user_id=0,application_id=42~hmac=020ed9bceb5d5c6be5a6b2bd07acbfde0ee266e8d5f9616c899ce8b5f07f663e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 400, ExternalTrackId = "1756569627", Source = "Deezer", Title = "Matilda", ArtistId = 18, AlbumId = 27, DurationSeconds = 245, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/b/9/0/ab91d126ca9023039206c8951ebdeafc.mp3?hdnea=exp=1776617758~acl=/api/1/1/a/b/9/0/ab91d126ca9023039206c8951ebdeafc.mp3*~data=user_id=0,application_id=42~hmac=727fbd2cf4c64cf996a27e76a0b318e117b3f900795538cef5fb10b80e8568c9", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 401, ExternalTrackId = "1756569637", Source = "Deezer", Title = "Cinema", ArtistId = 18, AlbumId = 27, DurationSeconds = 243, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/1/3/0/c13cd14c5d294fca906fe524f9dbcaee.mp3?hdnea=exp=1776617759~acl=/api/1/1/c/1/3/0/c13cd14c5d294fca906fe524f9dbcaee.mp3*~data=user_id=0,application_id=42~hmac=d3b0de7742b48723ff14379cd59a10866ca6eb21fe811b70c8892a63853656c1", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 402, ExternalTrackId = "1756569647", Source = "Deezer", Title = "Daydreaming", ArtistId = 18, AlbumId = 27, DurationSeconds = 187, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/8/b/0/08bbafeb9fbcc2881adcdabed7b76f73.mp3?hdnea=exp=1776617759~acl=/api/1/1/0/8/b/0/08bbafeb9fbcc2881adcdabed7b76f73.mp3*~data=user_id=0,application_id=42~hmac=3638341091eb6750154910f0fa730f15d3ec8e90ed6d122ad20233c45ccdb12a", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 403, ExternalTrackId = "1756569657", Source = "Deezer", Title = "Keep Driving", ArtistId = 18, AlbumId = 27, DurationSeconds = 140, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/3/0/0/03019734859ed3e1d6d841c57e705bc0.mp3?hdnea=exp=1776617760~acl=/api/1/1/0/3/0/0/03019734859ed3e1d6d841c57e705bc0.mp3*~data=user_id=0,application_id=42~hmac=61c97e2a38ef0d8a9995dde0fb56bff3ed68e42c74a2e379b0e21ef721bce05c", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 404, ExternalTrackId = "1756569667", Source = "Deezer", Title = "Satellite", ArtistId = 18, AlbumId = 27, DurationSeconds = 218, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/c/7/0/dc71eec08ecf7447bb557d0fc04276cc.mp3?hdnea=exp=1776617761~acl=/api/1/1/d/c/7/0/dc71eec08ecf7447bb557d0fc04276cc.mp3*~data=user_id=0,application_id=42~hmac=ea9c542c0d8eadb84e62a84d93bb805d4c992877a3e1c81c99b2b952da6ec50f", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 405, ExternalTrackId = "1756569677", Source = "Deezer", Title = "Boyfriends", ArtistId = 18, AlbumId = 27, DurationSeconds = 194, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/a/8/a/0/a8a4cd6d7daa979f985be330543dc2d8.mp3?hdnea=exp=1776617761~acl=/api/1/1/a/8/a/0/a8a4cd6d7daa979f985be330543dc2d8.mp3*~data=user_id=0,application_id=42~hmac=0605d2aa32b70714b7c6f049793b15b196754a87975bc43262fae98db0774b05", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 406, ExternalTrackId = "1756569687", Source = "Deezer", Title = "Love Of My Life", ArtistId = 18, AlbumId = 27, DurationSeconds = 191, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/4/2/d/0/42dcd33d3088488004398b65b3ce8446.mp3?hdnea=exp=1776617762~acl=/api/1/1/4/2/d/0/42dcd33d3088488004398b65b3ce8446.mp3*~data=user_id=0,application_id=42~hmac=cb9089702e6e5b5d07bd6997b9ae93935f637ab674f86dabd564fe7d90920b92", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 407, ExternalTrackId = "360301941", Source = "Deezer", Title = "Meet Me in the Hallway", ArtistId = 18, AlbumId = 28, DurationSeconds = 228, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/8/a/6/0/8a6e1d867a0400014da6d1a416c2a682.mp3?hdnea=exp=1776617762~acl=/api/1/1/8/a/6/0/8a6e1d867a0400014da6d1a416c2a682.mp3*~data=user_id=0,application_id=42~hmac=e7a0723d15d32cbdfe85b177e7da992590fb808497d2be99d0bf32b5020947c9", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 408, ExternalTrackId = "360301951", Source = "Deezer", Title = "Sign of the Times", ArtistId = 18, AlbumId = 28, DurationSeconds = 340, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/5/5/1/0/551d8084025fa8648c30aed373171301.mp3?hdnea=exp=1776617763~acl=/api/1/1/5/5/1/0/551d8084025fa8648c30aed373171301.mp3*~data=user_id=0,application_id=42~hmac=2d0d32963dc79d970363747d17ef80cf6b2f66bcda16e4fdf7e0389681eea9f5", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 409, ExternalTrackId = "360301961", Source = "Deezer", Title = "Carolina", ArtistId = 18, AlbumId = 28, DurationSeconds = 189, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/d/a/3/0/da37e687defedb4d914ff493e1be44b1.mp3?hdnea=exp=1776617764~acl=/api/1/1/d/a/3/0/da37e687defedb4d914ff493e1be44b1.mp3*~data=user_id=0,application_id=42~hmac=f080adb4c582bbcbf54eb5184f282c65487f9c917f279912698bc385231ee35b", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 410, ExternalTrackId = "360301971", Source = "Deezer", Title = "Two Ghosts", ArtistId = 18, AlbumId = 28, DurationSeconds = 229, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/2/0/3/0/2036615b802c16ec05c077315cd85e3c.mp3?hdnea=exp=1776617764~acl=/api/1/1/2/0/3/0/2036615b802c16ec05c077315cd85e3c.mp3*~data=user_id=0,application_id=42~hmac=052b052b523feb4739869378c89ea52bab8807ba749626dfcea36864e375c1fd", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 411, ExternalTrackId = "360301981", Source = "Deezer", Title = "Sweet Creature", ArtistId = 18, AlbumId = 28, DurationSeconds = 224, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/8/6/0/1869be61513bc531a84827226a1e38f5.mp3?hdnea=exp=1776617765~acl=/api/1/1/1/8/6/0/1869be61513bc531a84827226a1e38f5.mp3*~data=user_id=0,application_id=42~hmac=23d4206f9d33f79bfc53137847ddb53ef68c741e7cea86c8cfbbd93bed9695f1", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 412, ExternalTrackId = "360301991", Source = "Deezer", Title = "Only Angel", ArtistId = 18, AlbumId = 28, DurationSeconds = 291, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/0/9/9/0/099ee5a252368b907cc28c177a98e42d.mp3?hdnea=exp=1776617765~acl=/api/1/1/0/9/9/0/099ee5a252368b907cc28c177a98e42d.mp3*~data=user_id=0,application_id=42~hmac=772730ae7695262289b2413de9df380874283cb52517c6d04ca35223ff78c3ee", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 413, ExternalTrackId = "360302001", Source = "Deezer", Title = "Kiwi", ArtistId = 18, AlbumId = 28, DurationSeconds = 176, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/1/9/2/0/19270c21b29e92873d567f405fcbe895.mp3?hdnea=exp=1776617766~acl=/api/1/1/1/9/2/0/19270c21b29e92873d567f405fcbe895.mp3*~data=user_id=0,application_id=42~hmac=15ac564b5f4cb24a143326636e71700d89abbbd5c3889d3fea3d9e4c18ba3dff", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 414, ExternalTrackId = "360302011", Source = "Deezer", Title = "Ever Since New York", ArtistId = 18, AlbumId = 28, DurationSeconds = 253, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/b/e/0/cbe3838c7995957318248862616409c0.mp3?hdnea=exp=1776617767~acl=/api/1/1/c/b/e/0/cbe3838c7995957318248862616409c0.mp3*~data=user_id=0,application_id=42~hmac=acb0c43d39df1c199f57d8f7f11bcbc424945d430b0a92799e7617399a018d32", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 415, ExternalTrackId = "360302021", Source = "Deezer", Title = "Woman", ArtistId = 18, AlbumId = 28, DurationSeconds = 278, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/3/2/f/0/32fa33f9b27f60aa87207f6235f30967.mp3?hdnea=exp=1776617767~acl=/api/1/1/3/2/f/0/32fa33f9b27f60aa87207f6235f30967.mp3*~data=user_id=0,application_id=42~hmac=58216ff36f311fd6f84804ad617b191e208d64673457204a05dd3bf32c7d0707", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 416, ExternalTrackId = "360302031", Source = "Deezer", Title = "From the Dining Table", ArtistId = 18, AlbumId = 28, DurationSeconds = 211, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/9/7/a/0/97a9958024f29c72b4dade41f474e8e5.mp3?hdnea=exp=1776617768~acl=/api/1/1/9/7/a/0/97a9958024f29c72b4dade41f474e8e5.mp3*~data=user_id=0,application_id=42~hmac=ab4be41936b02d552cada7aa354fc30512ba4df0721a465f9b64b67db630074e", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 417, ExternalTrackId = "3791401032", Source = "Deezer", Title = "Aperture", ArtistId = 18, AlbumId = 29, DurationSeconds = 311, PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/7/1/2/0/71262a34dd461df51ec4fd72c55d1cb5.mp3?hdnea=exp=1776617769~acl=/api/1/1/7/1/2/0/71262a34dd461df51ec4fd72c55d1cb5.mp3*~data=user_id=0,application_id=42~hmac=e985a5354308713c262d001c3f15f630aac6738896bcfdd0a6da298e7bce7b91", CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fee004942feff253f7bbca63740ab519/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2026, 1, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 23), CreatedAt = new DateTime(2020, 1, 22) }

        );

            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, ExternalGenreId = "165", Source = "Deezer", Name = "R&B", CreatedAt = new DateTime(2020, 3, 25) },
                new Genre { Id = 2, ExternalGenreId = "116", Source = "Deezer", Name = "Rap/Hip Hop", CreatedAt = new DateTime(2020, 3, 25) },
                new Genre { Id = 3, ExternalGenreId = "85", Source = "Deezer", Name = "Alternative", CreatedAt = new DateTime(2020, 3, 25) },
                new Genre { Id = 4, ExternalGenreId = "132", Source = "Deezer", Name = "Pop", CreatedAt = new DateTime(2020, 3, 25) },
                new Genre { Id = 5, ExternalGenreId = "106", Source = "Deezer", Name = "Electro", CreatedAt = new DateTime(2020, 3, 25) },
                new Genre { Id = 6, ExternalGenreId = "134", Source = "Deezer", Name = "International Pop", CreatedAt = new DateTime(2020, 3, 25) },
                new Genre { Id = 7, ExternalGenreId = "152", Source = "Deezer", Name = "Rock", CreatedAt = new DateTime(2020, 3, 25) },
                new Genre { Id = 8, ExternalGenreId = "113", Source = "Deezer", Name = "Dance", CreatedAt = new DateTime(2020, 3, 25) }
            );

            modelBuilder.Entity<AlbumGenre>().HasData(
                new AlbumGenre { Id = 1, AlbumId = 1, GenreId = 1, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 2, AlbumId = 2, GenreId = 2, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 3, AlbumId = 3, GenreId = 2, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 4, AlbumId = 4, GenreId = 2, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 5, AlbumId = 5, GenreId = 3, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 6, AlbumId = 6, GenreId = 3, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 7, AlbumId = 7, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 8, AlbumId = 8, GenreId = 3, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 9, AlbumId = 9, GenreId = 5, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 10, AlbumId = 10, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 11, AlbumId = 11, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 12, AlbumId = 12, GenreId = 6, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 13, AlbumId = 12, GenreId = 7, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 14, AlbumId = 13, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 15, AlbumId = 14, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 16, AlbumId = 15, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 17, AlbumId = 16, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 18, AlbumId = 17, GenreId = 2, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 19, AlbumId = 18, GenreId = 3, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 20, AlbumId = 19, GenreId = 3, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 21, AlbumId = 20, GenreId = 3, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 22, AlbumId = 21, GenreId = 5, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 23, AlbumId = 22, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 24, AlbumId = 23, GenreId = 2, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 25, AlbumId = 24, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 26, AlbumId = 25, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 27, AlbumId = 26, GenreId = 8, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 28, AlbumId = 27, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 29, AlbumId = 28, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 30, AlbumId = 29, GenreId = 3, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 31, AlbumId = 29, GenreId = 8, CreatedAt = new DateTime(2020, 3, 25) },
                new AlbumGenre { Id = 32, AlbumId = 29, GenreId = 4, CreatedAt = new DateTime(2020, 3, 25) }
            );


            var additionalUsers = GenerateAdditionalUsers(userHash, 300);
            var allUsers = baseUsers.Concat(additionalUsers).ToList();

            modelBuilder.Entity<User>().HasData(allUsers);

            modelBuilder.Entity<UserRole>().HasData(
                GenerateUserRoles(allUsers.Count)
            );

            modelBuilder.Entity<SubscriptionPlan>().HasData(
                new SubscriptionPlan
                {
                    Id = 1,
                    Name = "Basic account",
                    Description = "Free account with limited features.",
                    Price = 0,
                    DurationDays = 0,
                    IsActive = true,
                },
                new SubscriptionPlan
                {
                    Id = 2,
                    Name = "Premium",
                    Description = "Premium account with full access.",
                    Price = 9.99f,
                    DurationDays = 30,
                    IsActive = true,
                }
            );

            var allSeedUsers = allUsers;

            // ======================= SUBSCRIPTIONS =======================
            var subscriptions = new List<Subscription>();
            int subscriptionId = 1;

            foreach (var user in allUsers)
            {
                if (user.Id == 1 || user.Id == 2)
                    continue;

                int planId =
                    user.Id % 3 == 0 ? 2 :
                    user.Id % 5 == 0 ? 2 :
                    1;

                subscriptions.Add(new Subscription
                {
                    Id = subscriptionId++,
                    UserId = user.Id,
                    SubscriptionPlanId = planId,
                    StartDate = user.JoinDate,
                    ExpiryDate = planId == 1 ? null : user.JoinDate.AddMonths(1),
                    IsActive = true
                });
            }

            modelBuilder.Entity<Subscription>().HasData(subscriptions);


            // ======================= PAYMENTS =======================
            var payments = new List<Payment>();
            int paymentId = 1;

            foreach (var sub in subscriptions)
            {
                if (sub.SubscriptionPlanId == 1)
                    continue;

                payments.Add(new Payment
                {
                    Id = paymentId++,
                    SubscriptionId = sub.Id,
                    PaymentAmount = 9.99f,
                    PaymentStatus = "Paid",
                    PaymentDate = sub.StartDate,
                    PaymentMethod = "Stripe",
                    CreatedAt = sub.StartDate,
                    PaidAt = sub.StartDate,
                    FailureReason = null,
                    StripePaymentIntentId = $"pi_seed_{sub.Id}"
                });
            }

            modelBuilder.Entity<Payment>().HasData(payments);

            var playlists = GeneratePlaylists(allUsers, subscriptions, 1);

            modelBuilder.Entity<Playlist>().HasData(playlists);

            modelBuilder.Entity<PlaylistSong>().HasData(
                GeneratePlaylistSongs(playlists, 1)
            );
            var playHistories = GeneratePlayHistories(allUsers.Count, 417, 2000);

            modelBuilder.Entity<PlayHistory>().HasData(playHistories);

            modelBuilder.Entity<Question>().HasData(

    new Question
    {
        Id = 1,
        UserId = 2,
        Title = "How do I add a song to a playlist?",
        Content = "I cannot find the option to add a song to a playlist.",
        Status = "Answered",
        Answer = "Click the three dots next to the song and choose 'Add to playlist'.",
        CreatedAt = new DateTime(2026, 3, 20, 10, 15, 0),
        AnsweredAt = new DateTime(2026, 3, 20, 11, 0, 0)
    },

    new Question
    {
        Id = 2,
        UserId = 3,
        Title = "Why is search not working for me?",
        Content = "When I search for songs, it does not return results.",
        Status = "Answered",
        Answer = "Check your internet connection and try again.",
        CreatedAt = new DateTime(2026, 3, 21, 14, 30, 0),
        AnsweredAt = new DateTime(2026, 3, 21, 15, 10, 0)
    },

    new Question
    {
        Id = 3,
        UserId = 4,
        Title = "How do I change my email?",
        Content = "I want to change the email on my profile.",
        Status = "Pending",
        Answer = null,
        CreatedAt = new DateTime(2026, 3, 22, 9, 45, 0),
        AnsweredAt = null
    },

    new Question
    {
        Id = 4,
        UserId = 5,
        Title = "Premium subscription is not working",
        Content = "I bought premium but do not have access to premium features.",
        Status = "Pending",
        Answer = null,
        CreatedAt = new DateTime(2026, 3, 23, 18, 20, 0),
        AnsweredAt = null
    }
);

            modelBuilder.Entity<Answer>().HasData(

                new Answer
                {
                    Id = 1,
                    QuestionId = 1,
                    AdminId = 1,
                    Message = "Click the three dots next to the song and choose 'Add to playlist'.",
                    CreatedAt = new DateTime(2026, 3, 20, 11, 0, 0)
                },

                new Answer
                {
                    Id = 2,
                    QuestionId = 2,
                    AdminId = 1,
                    Message = "Check your internet connection and try again.",
                    CreatedAt = new DateTime(2026, 3, 21, 15, 10, 0)
                },

                new Answer
                {
                    Id = 3,
                    QuestionId = 1,
                    AdminId = 1,
                    Message = "Ako i dalje ne radi, probaj restartovati aplikaciju.",
                    CreatedAt = new DateTime(2026, 3, 20, 11, 5, 0)
                }
            );


        }



        private static List<User> GenerateAdditionalUsers(string userHash, int additionalCount)
        {
            var random = new Random(20260311);
            var users = new List<User>();

            int startId = 12;
            int currentId = startId;

            var usedFullNames = new HashSet<string>();

            void AddUser(int year, int month)
            {
                string firstName;
                string lastName;
                string fullNameKey;

                do
                {
                    firstName = FirstNames[random.Next(FirstNames.Length)];
                    lastName = LastNames[random.Next(LastNames.Length)];
                    fullNameKey = $"{firstName} {lastName}".ToLower();
                }
                while (!usedFullNames.Add(fullNameKey));

                int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);

                var username =
                    $"{NormalizeForUsername(firstName)}{NormalizeForUsername(lastName)}{currentId}";

                users.Add(new User
                {
                    Id = currentId,
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    Password = string.Empty,
                    PasswordHash = userHash,
                    Email = $"{username}@grooveon.com",
                    UserImage = GetLoremPicsumImage("user", currentId),
                    DateOfBirth = new DateTime(
                        random.Next(1995, 2006),
                        random.Next(1, 13),
                        random.Next(1, 28)
                    ),
                    PhoneNumber = $"061{random.Next(100000, 999999)}",
                    IsActive = true,
                    JoinDate = new DateTime(year, month, day),
                    LastLogin = null
                });

                currentId++;
            }

            int guaranteedApril2026 = Math.Min(20, additionalCount / 10);
            int guaranteedMay2026 = Math.Min(20, additionalCount / 10);

            for (int i = 0; i < guaranteedApril2026; i++)
                AddUser(2026, 4);

            for (int i = 0; i < guaranteedMay2026; i++)
                AddUser(2026, 5);

            int remaining = additionalCount - users.Count;

            for (int i = 0; i < remaining; i++)
            {
                int year;
                int month;

                int roll = random.Next(100);

                if (roll < 20)
                {
                    year = 2026;
                    month = 4;
                }
                else if (roll < 40)
                {
                    year = 2026;
                    month = 5;
                }
                else
                {
                    year = random.Next(0, 2) == 0 ? 2025 : 2026;
                    month = random.Next(1, 13);
                }

                AddUser(year, month);
            }

            return users;
        }

        private static List<Playlist> GeneratePlaylists(
    List<User> users,
    List<Subscription> subscriptions,
    int startId)
        {
            var random = new Random(20260428);
            var playlists = new List<Playlist>();
            int playlistId = startId;

            var playlistNames = new[]
            {
        "Late Night Vibes",
        "Workout Energy",
        "Road Trip Mix",
        "Chill Zone",
        "Focus Beats",
        "Throwback Hits",
        "Weekend Mood",
        "Daily Rotation",
        "Heartbreak Sessions",
        "Party Starters",
        "Morning Boost",
        "Deep Focus",
        "Rap Essentials",
        "Pop Favorites",
        "Rock Classics",
        "Electronic Flow",
        "Summer Drive",
        "Acoustic Mood",
        "Gym Mode",
        "After Hours"
    };

            foreach (var user in users)
            {
                if (user.Id == 1 || user.Id == 2)
                    continue;

                var userSubscription = subscriptions
                    .FirstOrDefault(x => x.UserId == user.Id && x.IsActive);

                var isBasicAccount =
                    userSubscription == null ||
                    userSubscription.SubscriptionPlanId == 1;

                int playlistCount = isBasicAccount
                    ? random.Next(1, 4)      // Basic: 1 do 3 playliste
                    : random.Next(4, 9);     // Premium: 4 do 8 playlisti

                for (int i = 0; i < playlistCount; i++)
                {
                    var baseName = playlistNames[random.Next(playlistNames.Length)];

                    playlists.Add(new Playlist
                    {
                        Id = playlistId++,
                        UserId = user.Id,
                        Name = $"{baseName} #{i + 1}",
                        Description = GetPlaylistDescription(baseName),
                        IsPublic = random.Next(100) < 45,
                        CoverImageUrl = GetLoremPicsumImage("playlist", playlistId),
                        CreatedAt = user.JoinDate.AddDays(random.Next(1, 60))
                    });
                }
            }

            return playlists;
        }

        private static List<PlaylistSong> GeneratePlaylistSongs(
            List<Playlist> playlists,
            int startId)
        {
            var random = new Random(20260429);
            var playlistSongs = new List<PlaylistSong>();
            int playlistSongId = startId;

            const int minSongId = 1;
            const int maxSongId = 120;
            // If you have fewer songs, reduce maxSongId.
            // If you have more songs, feel free to increase it.

            foreach (var playlist in playlists)
            {
                int songCount = random.Next(5, 16);

                var songIds = new HashSet<int>();

                while (songIds.Count < songCount)
                {
                    songIds.Add(random.Next(minSongId, maxSongId + 1));
                }

                foreach (var songId in songIds)
                {
                    playlistSongs.Add(new PlaylistSong
                    {
                        Id = playlistSongId++,
                        PlaylistId = playlist.Id,
                        SongId = songId,
                        AddedAt = playlist.CreatedAt.AddDays(random.Next(1, 20))
                    });
                }
            }

            return playlistSongs;
        }

        private static string GetPlaylistDescription(string name)
        {
            return name switch
            {
                "Late Night Vibes" => "Songs for late night listening sessions.",
                "Workout Energy" => "High energy tracks for training and movement.",
                "Road Trip Mix" => "A playlist made for long drives.",
                "Chill Zone" => "Relaxed songs for easy listening.",
                "Focus Beats" => "Music for studying, coding and focus.",
                "Throwback Hits" => "Older favorites that still sound fresh.",
                "Weekend Mood" => "Tracks for the weekend mood.",
                "Daily Rotation" => "Songs played often during the day.",
                "Heartbreak Sessions" => "Emotional songs for slower moments.",
                "Party Starters" => "Tracks made for parties and group listening.",
                "Morning Boost" => "Music to start the day with energy.",
                "Deep Focus" => "Calm tracks for deep work.",
                "Rap Essentials" => "Rap tracks selected for daily listening.",
                "Pop Favorites" => "Popular songs for every mood.",
                "Rock Classics" => "Rock songs that always hit.",
                "Electronic Flow" => "Electronic songs with smooth energy.",
                "Summer Drive" => "Songs for warm weather and open roads.",
                "Acoustic Mood" => "Soft acoustic songs.",
                "Gym Mode" => "Strong tracks for gym sessions.",
                "After Hours" => "Darker and smoother night tracks.",
                _ => "Custom GrooveOn playlist."
            };
        }

        private static List<UserRole> GenerateUserRoles(int totalUsers)
        {
            var userRoles = new List<UserRole>();
            int roleIdCounter = 1;

            userRoles.Add(new UserRole
            {
                Id = roleIdCounter++,
                UserId = 1,
                RoleId = 1,
                DateAssigned = new DateTime(2026, 3, 10)
            });

            userRoles.Add(new UserRole
            {
                Id = roleIdCounter++,
                UserId = 2,
                RoleId = 1,
                DateAssigned = new DateTime(2026, 3, 10)
            });

            for (int userId = 3; userId <= totalUsers; userId++)
            {
                userRoles.Add(new UserRole
                {
                    Id = roleIdCounter++,
                    UserId = userId,
                    RoleId = 2,
                    DateAssigned = new DateTime(2026, 3, 10)
                });
            }

            return userRoles;
        }

        private static List<PlayHistory> GeneratePlayHistories(
     int totalUsers,
     int totalSongs,
     int totalPlayCount)
        {
            var random = new Random(20260313);
            var playHistories = new List<PlayHistory>();
            int playHistoryId = 1;

            int firstRegularUserId = 3;

            var popularSongIds = new List<int>
    {
        9, 11, 26, 34, 42, 65, 82, 101,
        126, 151, 188, 216, 245, 268, 297, 308
    };

            DateTime RandomDateInMonth(int year, int month)
            {
                int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);
                int hour = random.Next(0, 24);
                int minute = random.Next(0, 60);
                int second = random.Next(0, 60);

                return new DateTime(year, month, day, hour, minute, second);
            }

            DateTime RandomDateInYear(int year)
            {
                int month = random.Next(1, 13);
                return RandomDateInMonth(year, month);
            }

            DateTime RandomAllowedDate()
            {
                if (random.NextDouble() < 0.40)
                {
                    return RandomDateInYear(2025);
                }

                int month = random.Next(1, 6); // samo januar-maj 2026
                return RandomDateInMonth(2026, month);
            }

            int PickSongId()
            {
                if (random.NextDouble() < 0.65)
                {
                    return popularSongIds[random.Next(popularSongIds.Count)];
                }

                return random.Next(1, totalSongs + 1);
            }

            // Garantovani podaci samo za januar-maj 2026
            for (int month = 1; month <= 5; month++)
            {
                const int guaranteedPlaysForMonth = 90;

                for (int i = 0; i < guaranteedPlaysForMonth; i++)
                {
                    int userId = random.Next(firstRegularUserId, totalUsers + 1);

                    playHistories.Add(new PlayHistory
                    {
                        Id = playHistoryId++,
                        UserId = userId,
                        SongId = PickSongId(),
                        PlayedAt = RandomDateInMonth(2026, month)
                    });
                }
            }

            // Dodatni play-evi po useru, ali samo 2025 ili januar-maj 2026
            for (int userId = firstRegularUserId; userId <= totalUsers; userId++)
            {
                int playsForUser = random.Next(12, 28);

                for (int i = 0; i < playsForUser; i++)
                {
                    playHistories.Add(new PlayHistory
                    {
                        Id = playHistoryId++,
                        UserId = userId,
                        SongId = PickSongId(),
                        PlayedAt = RandomAllowedDate()
                    });
                }
            }

            // Dopuna do totalPlayCount, opet samo dozvoljeni periodi
            while (playHistories.Count < totalPlayCount)
            {
                int userId = random.Next(firstRegularUserId, totalUsers + 1);

                playHistories.Add(new PlayHistory
                {
                    Id = playHistoryId++,
                    UserId = userId,
                    SongId = PickSongId(),
                    PlayedAt = RandomAllowedDate()
                });
            }

            playHistories = playHistories
                .OrderBy(x => x.PlayedAt)
                .Select((x, index) => new PlayHistory
                {
                    Id = index + 1,
                    UserId = x.UserId,
                    SongId = x.SongId,
                    PlayedAt = x.PlayedAt
                })
                .ToList();

            return playHistories;
        }

        private static List<Subscription> GenerateSubscriptions(List<User> users, int startId)
        {
            var random = new Random(20260312);
            var subscriptions = new List<Subscription>();
            int subscriptionId = startId;

            foreach (var user in users)
            {
                if (user.Id == 1 || user.Id == 2)
                    continue;

                int planId;

                if (user.JoinDate.Year == 2025)
                {
                    planId = random.Next(100) < 55 ? 2 : 1;
                }
                else
                {
                    planId = random.Next(100) < 70 ? 2 : 1;
                }

                var startDate = user.JoinDate;

                subscriptions.Add(new Subscription
                {
                    Id = subscriptionId++,
                    UserId = user.Id,
                    SubscriptionPlanId = planId,
                    StartDate = startDate,
                    ExpiryDate = planId == 1 ? null : startDate.AddMonths(random.Next(1, 4)),
                    IsActive = planId == 1 || random.Next(100) < 85
                });
            }

            return subscriptions;
        }

        private static List<Payment> GeneratePayments(List<Subscription> subscriptions, int startId)
        {
            var random = new Random(20260429);
            var payments = new List<Payment>();
            int paymentId = startId;

            string[] methods = { "Card", "Stripe", "PayPal" };

            foreach (var subscription in subscriptions)
            {
                if (subscription.SubscriptionPlanId != 2)
                    continue;

                var numberOfPayments = subscription.StartDate.Year == 2025
                    ? random.Next(2, 6)
                    : random.Next(1, 4);

                for (int i = 0; i < numberOfPayments; i++)
                {
                    var paymentDate = subscription.StartDate.AddMonths(i);

                    if (paymentDate.Year > 2026)
                        break;

                    var statusRoll = random.Next(100);

                    string status;
                    DateTime? paidAt;
                    string? failureReason;

                    if (statusRoll < 82)
                    {
                        status = "Paid";
                        paidAt = paymentDate;
                        failureReason = null;
                    }
                    else if (statusRoll < 92)
                    {
                        status = "Pending";
                        paidAt = null;
                        failureReason = null;
                    }
                    else
                    {
                        status = "Failed";
                        paidAt = null;
                        failureReason = "Payment was declined.";
                    }

                    payments.Add(new Payment
                    {
                        Id = paymentId++,
                        SubscriptionId = subscription.Id,
                        PaymentStatus = status,
                        StripePaymentIntentId = $"pi_seed_{subscription.Id}_{i + 1}",
                        CreatedAt = paymentDate.AddDays(-random.Next(0, 3)),
                        PaidAt = paidAt,
                        FailureReason = failureReason,
                        PaymentMethod = methods[random.Next(methods.Length)],
                        PaymentAmount = 9.99f,
                        PaymentDate = paymentDate
                    });
                }
            }

            return payments;
        }


        private static int GetMaxAllowedMonthForYear(int year)
        {
            var today = DateTime.Today;

            if (year < today.Year)
                return 12;

            if (year == today.Year)
                return today.Month - 1;

            return 0;
        }
    }
}