using GrooveOn.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GrooveOn.Services.Database
{
    public class GrooveOnDbContext : DbContext
    {
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
        public DbSet<PlayHistory> ListeningHistories { get; set; }

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
                    Name = "Korisnik",
                    Description = "Standardni korisnik aplikacije",
                    CreatedAt = new DateTime(2026, 3, 10),
                    IsActive = true
                }
            );

            UserHelper.CreatePasswordHash("Admin123!", out string adminHash, out string adminSalt);
            UserHelper.CreatePasswordHash("User123!", out string userHash, out string userSalt);

            var baseUsers = new List<User>
    {
        new User
        {
            Id = 1,
            FirstName = "Marko",
            LastName = "Petrović",
            Username = "markopetrovic01",
            Password = string.Empty,
            PasswordHash = adminHash,
            PasswordSalt = adminSalt,
            Email = "admin@grooveon.com",
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
            FirstName = "Nikola",
            LastName = "Jovanović",
            Username = "nikolajovanovic02",
            Password = string.Empty,
            PasswordHash = adminHash,
            PasswordSalt = adminSalt,
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
            LastName = "Hadžić",
            Username = "amarhadzic03",
            Password = string.Empty,
            PasswordHash = userHash,
            PasswordSalt = userSalt,
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
            LastName = "Kovačević",
            Username = "lejlakovacevic04",
            Password = string.Empty,
            PasswordHash = userHash,
            PasswordSalt = userSalt,
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
            LastName = "Mehić",
            Username = "benjaminmehic05",
            Password = string.Empty,
            PasswordHash = userHash,
            PasswordSalt = userSalt,
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
            LastName = "Delić",
            Username = "saradelic06",
            Password = string.Empty,
            PasswordHash = userHash,
            PasswordSalt = userSalt,
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
            LastName = "Karić",
            Username = "adnankaric07",
            Password = string.Empty,
            PasswordHash = userHash,
            PasswordSalt = userSalt,
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
            LastName = "Selimović",
            Username = "eminaselimovic08",
            Password = string.Empty,
            PasswordHash = userHash,
            PasswordSalt = userSalt,
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
            LastName = "Mujić",
            Username = "harismujic09",
            Password = string.Empty,
            PasswordHash = userHash,
            PasswordSalt = userSalt,
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
            LastName = "Alić",
            Username = "jasminalic10",
            Password = string.Empty,
            PasswordHash = userHash,
            PasswordSalt = userSalt,
            Email = "user8@grooveon.com",
            UserImage = null,
            DateOfBirth = new DateTime(2000, 8, 17),
            PhoneNumber = "061111120",
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
            new Song { Id = 1, ExternalTrackId = "908604532", Source = "Deezer", Title = "Alone Again", ArtistId = 1, AlbumId = 1, DurationSeconds = 252, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 2, ExternalTrackId = "908604542", Source = "Deezer", Title = "Too Late", ArtistId = 1, AlbumId = 1, DurationSeconds = 239, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 3, ExternalTrackId = "908604552", Source = "Deezer", Title = "Hardest To Love", ArtistId = 1, AlbumId = 1, DurationSeconds = 211, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 4, ExternalTrackId = "908604562", Source = "Deezer", Title = "Scared To Live", ArtistId = 1, AlbumId = 1, DurationSeconds = 191, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 5, ExternalTrackId = "908604572", Source = "Deezer", Title = "Snowchild", ArtistId = 1, AlbumId = 1, DurationSeconds = 247, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 6, ExternalTrackId = "908604582", Source = "Deezer", Title = "Escape From LA", ArtistId = 1, AlbumId = 1, DurationSeconds = 355, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 7, ExternalTrackId = "908604592", Source = "Deezer", Title = "Heartless", ArtistId = 1, AlbumId = 1, DurationSeconds = 200, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 8, ExternalTrackId = "908604602", Source = "Deezer", Title = "Faith", ArtistId = 1, AlbumId = 1, DurationSeconds = 283, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 9, ExternalTrackId = "908604612", Source = "Deezer", Title = "Blinding Lights", ArtistId = 1, AlbumId = 1, DurationSeconds = 200, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 10, ExternalTrackId = "908604622", Source = "Deezer", Title = "In Your Eyes", ArtistId = 1, AlbumId = 1, DurationSeconds = 237, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 11, ExternalTrackId = "908604632", Source = "Deezer", Title = "Save Your Tears", ArtistId = 1, AlbumId = 1, DurationSeconds = 215, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 12, ExternalTrackId = "908604642", Source = "Deezer", Title = "Repeat After Me (Interlude)", ArtistId = 1, AlbumId = 1, DurationSeconds = 195, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 13, ExternalTrackId = "908604652", Source = "Deezer", Title = "After Hours", ArtistId = 1, AlbumId = 1, DurationSeconds = 362, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 14, ExternalTrackId = "908604662", Source = "Deezer", Title = "Until I Bleed Out", ArtistId = 1, AlbumId = 1, DurationSeconds = 190, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fd00ebd6d30d7253f813dba3bb1c66a9/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2020, 3, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 21), CreatedAt = new DateTime(2020, 3, 20) },

            new Song { Id = 15, ExternalTrackId = "124603248", Source = "Deezer", Title = "Keep The Family Close", ArtistId = 2, AlbumId = 2, DurationSeconds = 331, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 16, ExternalTrackId = "124603250", Source = "Deezer", Title = "9", ArtistId = 2, AlbumId = 2, DurationSeconds = 256, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 17, ExternalTrackId = "124603252", Source = "Deezer", Title = "U With Me?", ArtistId = 2, AlbumId = 2, DurationSeconds = 297, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 18, ExternalTrackId = "124603254", Source = "Deezer", Title = "Feel No Ways", ArtistId = 2, AlbumId = 2, DurationSeconds = 241, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 19, ExternalTrackId = "124603256", Source = "Deezer", Title = "Hype", ArtistId = 2, AlbumId = 2, DurationSeconds = 209, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 20, ExternalTrackId = "124603258", Source = "Deezer", Title = "Weston Road Flows", ArtistId = 2, AlbumId = 2, DurationSeconds = 253, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 21, ExternalTrackId = "124603260", Source = "Deezer", Title = "Redemption", ArtistId = 2, AlbumId = 2, DurationSeconds = 334, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 22, ExternalTrackId = "124603262", Source = "Deezer", Title = "With You", ArtistId = 2, AlbumId = 2, DurationSeconds = 195, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 23, ExternalTrackId = "124603264", Source = "Deezer", Title = "Faithful", ArtistId = 2, AlbumId = 2, DurationSeconds = 290, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 24, ExternalTrackId = "124603266", Source = "Deezer", Title = "Still Here", ArtistId = 2, AlbumId = 2, DurationSeconds = 190, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 25, ExternalTrackId = "124603268", Source = "Deezer", Title = "Controlla", ArtistId = 2, AlbumId = 2, DurationSeconds = 245, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 26, ExternalTrackId = "124603270", Source = "Deezer", Title = "One Dance", ArtistId = 2, AlbumId = 2, DurationSeconds = 174, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 27, ExternalTrackId = "124603272", Source = "Deezer", Title = "Grammys", ArtistId = 2, AlbumId = 2, DurationSeconds = 220, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 28, ExternalTrackId = "124603274", Source = "Deezer", Title = "Childs Play", ArtistId = 2, AlbumId = 2, DurationSeconds = 241, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 29, ExternalTrackId = "124603276", Source = "Deezer", Title = "Pop Style", ArtistId = 2, AlbumId = 2, DurationSeconds = 213, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 30, ExternalTrackId = "124603278", Source = "Deezer", Title = "Too Good", ArtistId = 2, AlbumId = 2, DurationSeconds = 263, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 31, ExternalTrackId = "124603280", Source = "Deezer", Title = "Summers Over Interlude", ArtistId = 2, AlbumId = 2, DurationSeconds = 106, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 32, ExternalTrackId = "124603282", Source = "Deezer", Title = "Fire & Desire", ArtistId = 2, AlbumId = 2, DurationSeconds = 238, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 33, ExternalTrackId = "124603284", Source = "Deezer", Title = "Views", ArtistId = 2, AlbumId = 2, DurationSeconds = 312, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 34, ExternalTrackId = "124603286", Source = "Deezer", Title = "Hotline Bling", ArtistId = 2, AlbumId = 2, DurationSeconds = 267, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/56bdb7a86a27fadb96332c0c8f1b8e81/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 5, 6), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 7), CreatedAt = new DateTime(2020, 5, 6) },

            new Song { Id = 35, ExternalTrackId = "2386586015", Source = "Deezer", Title = "HYAENA", ArtistId = 3, AlbumId = 3, DurationSeconds = 222, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 36, ExternalTrackId = "2386586025", Source = "Deezer", Title = "THANK GOD", ArtistId = 3, AlbumId = 3, DurationSeconds = 184, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 37, ExternalTrackId = "2386586035", Source = "Deezer", Title = "MODERN JAM feat. Teezo Touchdown", ArtistId = 3, AlbumId = 3, DurationSeconds = 255, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 38, ExternalTrackId = "2386586045", Source = "Deezer", Title = "MY EYES", ArtistId = 3, AlbumId = 3, DurationSeconds = 251, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 39, ExternalTrackId = "2386586055", Source = "Deezer", Title = "GOD'S COUNTRY", ArtistId = 3, AlbumId = 3, DurationSeconds = 127, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 40, ExternalTrackId = "2386586065", Source = "Deezer", Title = "SIRENS", ArtistId = 3, AlbumId = 3, DurationSeconds = 204, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 41, ExternalTrackId = "2386586075", Source = "Deezer", Title = "MELTDOWN (feat. Drake)", ArtistId = 3, AlbumId = 3, DurationSeconds = 246, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 42, ExternalTrackId = "2386586085", Source = "Deezer", Title = "FE!N (feat. Playboi Carti)", ArtistId = 3, AlbumId = 3, DurationSeconds = 191, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 43, ExternalTrackId = "2386586095", Source = "Deezer", Title = "DELRESTO (ECHOES) (feat. Beyoncé)", ArtistId = 3, AlbumId = 3, DurationSeconds = 274, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 44, ExternalTrackId = "2386586105", Source = "Deezer", Title = "I KNOW ?", ArtistId = 3, AlbumId = 3, DurationSeconds = 211, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 45, ExternalTrackId = "2386586115", Source = "Deezer", Title = "TOPIA TWINS (feat. Rob49 & 21 Savage)", ArtistId = 3, AlbumId = 3, DurationSeconds = 223, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 46, ExternalTrackId = "2386586125", Source = "Deezer", Title = "CIRCUS MAXIMUS (feat. The Weeknd)", ArtistId = 3, AlbumId = 3, DurationSeconds = 258, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 47, ExternalTrackId = "2386586135", Source = "Deezer", Title = "PARASAIL (feat. Young Thug)", ArtistId = 3, AlbumId = 3, DurationSeconds = 154, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 48, ExternalTrackId = "2386586145", Source = "Deezer", Title = "SKITZO (feat. Young Thug)", ArtistId = 3, AlbumId = 3, DurationSeconds = 366, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 49, ExternalTrackId = "2386586155", Source = "Deezer", Title = "LOST FOREVER (feat. Westside Gunn)", ArtistId = 3, AlbumId = 3, DurationSeconds = 163, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 50, ExternalTrackId = "2386586165", Source = "Deezer", Title = "LOOOVE (feat. Kid Cudi)", ArtistId = 3, AlbumId = 3, DurationSeconds = 226, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 51, ExternalTrackId = "2386586175", Source = "Deezer", Title = "K-POP (feat. Bad Bunny & The Weeknd)", ArtistId = 3, AlbumId = 3, DurationSeconds = 185, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 52, ExternalTrackId = "2386586185", Source = "Deezer", Title = "TELEKINESIS (feat. SZA & Future)", ArtistId = 3, AlbumId = 3, DurationSeconds = 353, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 53, ExternalTrackId = "2386586195", Source = "Deezer", Title = "TIL FURTHER NOTICE (feat. James Blake & 21 Savage)", ArtistId = 3, AlbumId = 3, DurationSeconds = 314, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/6c91e64b7157f1332a4f6b0de9e4c714/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2023, 7, 28), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 29), CreatedAt = new DateTime(2020, 7, 28) },

            new Song { Id = 54, ExternalTrackId = "630827222", Source = "Deezer", Title = "Good Morning", ArtistId = 4, AlbumId = 4, DurationSeconds = 195, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 55, ExternalTrackId = "630827232", Source = "Deezer", Title = "Champion", ArtistId = 4, AlbumId = 4, DurationSeconds = 167, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 56, ExternalTrackId = "630827242", Source = "Deezer", Title = "Stronger", ArtistId = 4, AlbumId = 4, DurationSeconds = 312, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 57, ExternalTrackId = "630827252", Source = "Deezer", Title = "I Wonder", ArtistId = 4, AlbumId = 4, DurationSeconds = 243, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 58, ExternalTrackId = "630827262", Source = "Deezer", Title = "Good Life", ArtistId = 4, AlbumId = 4, DurationSeconds = 207, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 59, ExternalTrackId = "630827272", Source = "Deezer", Title = "Can't Tell Me Nothing", ArtistId = 4, AlbumId = 4, DurationSeconds = 274, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 60, ExternalTrackId = "630827282", Source = "Deezer", Title = "Barry Bonds", ArtistId = 4, AlbumId = 4, DurationSeconds = 204, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 61, ExternalTrackId = "630827292", Source = "Deezer", Title = "Drunk and Hot Girls", ArtistId = 4, AlbumId = 4, DurationSeconds = 313, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 62, ExternalTrackId = "630827302", Source = "Deezer", Title = "Flashing Lights", ArtistId = 4, AlbumId = 4, DurationSeconds = 237, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 63, ExternalTrackId = "630827312", Source = "Deezer", Title = "Everything I Am", ArtistId = 4, AlbumId = 4, DurationSeconds = 227, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 64, ExternalTrackId = "630827322", Source = "Deezer", Title = "The Glory", ArtistId = 4, AlbumId = 4, DurationSeconds = 212, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 65, ExternalTrackId = "630827332", Source = "Deezer", Title = "Homecoming", ArtistId = 4, AlbumId = 4, DurationSeconds = 203, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 66, ExternalTrackId = "630827342", Source = "Deezer", Title = "Big Brother", ArtistId = 4, AlbumId = 4, DurationSeconds = 287, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 67, ExternalTrackId = "630827352", Source = "Deezer", Title = "Good Night", ArtistId = 4, AlbumId = 4, DurationSeconds = 186, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/8c6578a2099561992fb7544e6826f767/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2007, 9, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 16), CreatedAt = new DateTime(2020, 9, 15) },

            new Song { Id = 68, ExternalTrackId = "14628993", Source = "Deezer", Title = "Foreword", ArtistId = 5, AlbumId = 5, DurationSeconds = 13, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 69, ExternalTrackId = "14628994", Source = "Deezer", Title = "Don't Stay", ArtistId = 5, AlbumId = 5, DurationSeconds = 187, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 70, ExternalTrackId = "14628995", Source = "Deezer", Title = "Somewhere I Belong", ArtistId = 5, AlbumId = 5, DurationSeconds = 213, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 71, ExternalTrackId = "14628996", Source = "Deezer", Title = "Lying from You", ArtistId = 5, AlbumId = 5, DurationSeconds = 175, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 72, ExternalTrackId = "14628997", Source = "Deezer", Title = "Hit the Floor", ArtistId = 5, AlbumId = 5, DurationSeconds = 164, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 73, ExternalTrackId = "14628998", Source = "Deezer", Title = "Easier to Run", ArtistId = 5, AlbumId = 5, DurationSeconds = 204, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 74, ExternalTrackId = "14628999", Source = "Deezer", Title = "Faint", ArtistId = 5, AlbumId = 5, DurationSeconds = 162, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 75, ExternalTrackId = "14629000", Source = "Deezer", Title = "Figure.09", ArtistId = 5, AlbumId = 5, DurationSeconds = 197, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 76, ExternalTrackId = "14629001", Source = "Deezer", Title = "Breaking the Habit", ArtistId = 5, AlbumId = 5, DurationSeconds = 196, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 77, ExternalTrackId = "14629002", Source = "Deezer", Title = "From the Inside", ArtistId = 5, AlbumId = 5, DurationSeconds = 175, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 78, ExternalTrackId = "14629003", Source = "Deezer", Title = "Nobody's Listening", ArtistId = 5, AlbumId = 5, DurationSeconds = 178, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 79, ExternalTrackId = "14629004", Source = "Deezer", Title = "Session", ArtistId = 5, AlbumId = 5, DurationSeconds = 144, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 80, ExternalTrackId = "14629005", Source = "Deezer", Title = "Numb", ArtistId = 5, AlbumId = 5, DurationSeconds = 187, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/882448ab63952aa16e502c82db2df160/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2003, 3, 24), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 25), CreatedAt = new DateTime(2020, 3, 24) },

            new Song { Id = 81, ExternalTrackId = "103052650", Source = "Deezer", Title = "Let It Happen", ArtistId = 6, AlbumId = 6, DurationSeconds = 469, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 82, ExternalTrackId = "103052652", Source = "Deezer", Title = "Nangs", ArtistId = 6, AlbumId = 6, DurationSeconds = 106, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 83, ExternalTrackId = "103052654", Source = "Deezer", Title = "The Moment", ArtistId = 6, AlbumId = 6, DurationSeconds = 255, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 84, ExternalTrackId = "103052656", Source = "Deezer", Title = "Yes I'm Changing", ArtistId = 6, AlbumId = 6, DurationSeconds = 270, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 85, ExternalTrackId = "103052658", Source = "Deezer", Title = "Eventually", ArtistId = 6, AlbumId = 6, DurationSeconds = 319, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 86, ExternalTrackId = "103052660", Source = "Deezer", Title = "Gossip", ArtistId = 6, AlbumId = 6, DurationSeconds = 55, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 87, ExternalTrackId = "103052662", Source = "Deezer", Title = "The Less I Know The Better", ArtistId = 6, AlbumId = 6, DurationSeconds = 217, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 88, ExternalTrackId = "103052664", Source = "Deezer", Title = "Past Life", ArtistId = 6, AlbumId = 6, DurationSeconds = 227, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 89, ExternalTrackId = "103052666", Source = "Deezer", Title = "Disciples", ArtistId = 6, AlbumId = 6, DurationSeconds = 106, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song { Id = 90, ExternalTrackId = "103052668", Source = "Deezer", Title = "'Cause I'm A Man", ArtistId = 6, AlbumId = 6, DurationSeconds = 243, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8bf49beb3e17ba2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 7, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 7, 18), CreatedAt = new DateTime(2020, 7, 17) },

            new Song
            {
                Id = 91,
                ExternalTrackId = "103052670",
                Source = "Deezer",
                Title = "Reality In Motion",
                ArtistId = 6,
                AlbumId = 6,
                DurationSeconds = 251,
                PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/c/2/4/0/103052670.mp3",
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
            PreviewUrl = "https://cdnt-preview.dzcdn.net/api/1/1/6/a/4/0/103052672.mp3",
            CoverUrl = "https://cdn-images.dzcdn.net/images/cover/de5b9b704cd4ec36f8df49beb3e17ba2/250x250-000000-80-0-0.jpg",
            ReleaseDate = new DateTime(2015, 7, 17),
            IsActive = true,
            LastSyncedAt = new DateTime(2020, 3, 25),
            CreatedAt = new DateTime(2020, 3, 25)
        },

            new Song { Id = 93, ExternalTrackId = "871688492", Source = "Deezer", Title = "State Of Grace", ArtistId = 7, AlbumId = 7, DurationSeconds = 296, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 94, ExternalTrackId = "871688502", Source = "Deezer", Title = "Red", ArtistId = 7, AlbumId = 7, DurationSeconds = 223, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 95, ExternalTrackId = "871688512", Source = "Deezer", Title = "Treacherous", ArtistId = 7, AlbumId = 7, DurationSeconds = 243, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 96, ExternalTrackId = "871688522", Source = "Deezer", Title = "I Knew You Were Trouble.", ArtistId = 7, AlbumId = 7, DurationSeconds = 219, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 97, ExternalTrackId = "871688532", Source = "Deezer", Title = "All Too Well", ArtistId = 7, AlbumId = 7, DurationSeconds = 329, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 98, ExternalTrackId = "871688552", Source = "Deezer", Title = "22", ArtistId = 7, AlbumId = 7, DurationSeconds = 232, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 99, ExternalTrackId = "871688562", Source = "Deezer", Title = "I Almost Do", ArtistId = 7, AlbumId = 7, DurationSeconds = 245, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 100, ExternalTrackId = "871688572", Source = "Deezer", Title = "We Are Never Ever Getting Back Together", ArtistId = 7, AlbumId = 7, DurationSeconds = 192, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 101, ExternalTrackId = "871688582", Source = "Deezer", Title = "Stay Stay Stay", ArtistId = 7, AlbumId = 7, DurationSeconds = 206, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 102, ExternalTrackId = "871688602", Source = "Deezer", Title = "The Last Time", ArtistId = 7, AlbumId = 7, DurationSeconds = 299, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 103, ExternalTrackId = "871688612", Source = "Deezer", Title = "Holy Ground", ArtistId = 7, AlbumId = 7, DurationSeconds = 203, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 104, ExternalTrackId = "871688622", Source = "Deezer", Title = "Sad Beautiful Tragic", ArtistId = 7, AlbumId = 7, DurationSeconds = 285, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 105, ExternalTrackId = "871688632", Source = "Deezer", Title = "The Lucky One", ArtistId = 7, AlbumId = 7, DurationSeconds = 240, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 106, ExternalTrackId = "871688642", Source = "Deezer", Title = "Everything Has Changed", ArtistId = 7, AlbumId = 7, DurationSeconds = 245, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 107, ExternalTrackId = "871688652", Source = "Deezer", Title = "Starlight", ArtistId = 7, AlbumId = 7, DurationSeconds = 221, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 108, ExternalTrackId = "871688662", Source = "Deezer", Title = "Begin Again", ArtistId = 7, AlbumId = 7, DurationSeconds = 238, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/d6f308f8329892e3f1eb105906aa77c2/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 10, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 23), CreatedAt = new DateTime(2020, 10, 22) },

            new Song { Id = 109, ExternalTrackId = "68097787", Source = "Deezer", Title = "Hotel California (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 391, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 110, ExternalTrackId = "68097788", Source = "Deezer", Title = "New Kid in Town (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 304, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 111, ExternalTrackId = "68097789", Source = "Deezer", Title = "Life in the Fast Lane (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 286, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 112, ExternalTrackId = "68097790", Source = "Deezer", Title = "Wasted Time (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 296, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 113, ExternalTrackId = "68097791", Source = "Deezer", Title = "Wasted Time (Reprise) (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 83, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 114, ExternalTrackId = "68097792", Source = "Deezer", Title = "Victim of Love (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 250, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 115, ExternalTrackId = "68097793", Source = "Deezer", Title = "Pretty Maids All in a Row (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 239, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 116, ExternalTrackId = "68097794", Source = "Deezer", Title = "Try and Love Again (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 311, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

        new Song
        {
            Id = 117,
            ExternalTrackId = "68097794",
            Source = "Deezer",
            Title = "The Last Resort (2013 Remaster)",
            ArtistId = 8,
            AlbumId = 8,
            DurationSeconds = 444,
            PreviewUrl = null,
            CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg",
            ReleaseDate = new DateTime(2006, 4, 3),
            IsActive = true,
            LastSyncedAt = new DateTime(2020, 4, 4),
            CreatedAt = new DateTime(2020, 4, 3)
        },

            new Song { Id = 118, ExternalTrackId = "68097795", Source = "Deezer", Title = "The Last Resort (2013 Remaster)", ArtistId = 8, AlbumId = 8, DurationSeconds = 444, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/ef02459f8e72f33acef71617a97d3999/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 4, 3), IsActive = true, LastSyncedAt = new DateTime(2020, 4, 4), CreatedAt = new DateTime(2020, 4, 3) },

            new Song { Id = 119, ExternalTrackId = "4315309", Source = "Deezer", Title = "The View From The Afternoon", ArtistId = 9, AlbumId = 9, DurationSeconds = 222, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 120, ExternalTrackId = "4315310", Source = "Deezer", Title = "I Bet You Look Good On The Dancefloor", ArtistId = 9, AlbumId = 9, DurationSeconds = 173, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 121, ExternalTrackId = "4315311", Source = "Deezer", Title = "Fake Tales Of San Francisco", ArtistId = 9, AlbumId = 9, DurationSeconds = 177, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 122, ExternalTrackId = "4315312", Source = "Deezer", Title = "Dancing Shoes", ArtistId = 9, AlbumId = 9, DurationSeconds = 141, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 123, ExternalTrackId = "4315313", Source = "Deezer", Title = "You Probably Couldn't See For The Lights But You Were Staring Straight At Me", ArtistId = 9, AlbumId = 9, DurationSeconds = 130, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 124, ExternalTrackId = "4315314", Source = "Deezer", Title = "Still Take You Home", ArtistId = 9, AlbumId = 9, DurationSeconds = 173, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 125, ExternalTrackId = "4315315", Source = "Deezer", Title = "Riot Van", ArtistId = 9, AlbumId = 9, DurationSeconds = 134, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 126, ExternalTrackId = "4315316", Source = "Deezer", Title = "Red Light Indicates Doors Are Secured", ArtistId = 9, AlbumId = 9, DurationSeconds = 143, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 127, ExternalTrackId = "4315317", Source = "Deezer", Title = "Mardy Bum", ArtistId = 9, AlbumId = 9, DurationSeconds = 175, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 128, ExternalTrackId = "4315318", Source = "Deezer", Title = "Perhaps Vampires Is A Bit Strong But...", ArtistId = 9, AlbumId = 9, DurationSeconds = 268, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 129, ExternalTrackId = "4315319", Source = "Deezer", Title = "When The Sun Goes Down", ArtistId = 9, AlbumId = 9, DurationSeconds = 202, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 130, ExternalTrackId = "4315320", Source = "Deezer", Title = "From The Ritz To The Rubble", ArtistId = 9, AlbumId = 9, DurationSeconds = 193, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 131, ExternalTrackId = "4315321", Source = "Deezer", Title = "A Certain Romance", ArtistId = 9, AlbumId = 9, DurationSeconds = 331, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/f7a0a1ca91431861989efe5a22aad557/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 2, 18), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 19), CreatedAt = new DateTime(2020, 2, 18) },

            new Song { Id = 132, ExternalTrackId = "75526533", Source = "Deezer", Title = "Intro", ArtistId = 10, AlbumId = 10, DurationSeconds = 65, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 133, ExternalTrackId = "75526534", Source = "Deezer", Title = "When A Fire Starts To Burn", ArtistId = 10, AlbumId = 10, DurationSeconds = 284, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 134, ExternalTrackId = "75526535", Source = "Deezer", Title = "Latch", ArtistId = 10, AlbumId = 10, DurationSeconds = 257, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 135, ExternalTrackId = "75526536", Source = "Deezer", Title = "For You", ArtistId = 10, AlbumId = 10, DurationSeconds = 269, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 136, ExternalTrackId = "75526537", Source = "Deezer", Title = "White Noise", ArtistId = 10, AlbumId = 10, DurationSeconds = 278, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 137, ExternalTrackId = "75526538", Source = "Deezer", Title = "Defeated No More", ArtistId = 10, AlbumId = 10, DurationSeconds = 368, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 138, ExternalTrackId = "75526539", Source = "Deezer", Title = "Stimulation", ArtistId = 10, AlbumId = 10, DurationSeconds = 320, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 139, ExternalTrackId = "75526540", Source = "Deezer", Title = "Voices", ArtistId = 10, AlbumId = 10, DurationSeconds = 249, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 140, ExternalTrackId = "75526541", Source = "Deezer", Title = "Second Chance", ArtistId = 10, AlbumId = 10, DurationSeconds = 151, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 141, ExternalTrackId = "75526542", Source = "Deezer", Title = "Grab Her!", ArtistId = 10, AlbumId = 10, DurationSeconds = 313, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 142, ExternalTrackId = "75526543", Source = "Deezer", Title = "You & Me", ArtistId = 10, AlbumId = 10, DurationSeconds = 266, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 143, ExternalTrackId = "75526544", Source = "Deezer", Title = "January", ArtistId = 10, AlbumId = 10, DurationSeconds = 355, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 144, ExternalTrackId = "75526545", Source = "Deezer", Title = "Confess To Me", ArtistId = 10, AlbumId = 10, DurationSeconds = 250, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 145, ExternalTrackId = "75526546", Source = "Deezer", Title = "Help Me Lose My Mind", ArtistId = 10, AlbumId = 10, DurationSeconds = 244, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 146, ExternalTrackId = "75526547", Source = "Deezer", Title = "Boiling", ArtistId = 10, AlbumId = 10, DurationSeconds = 227, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 147, ExternalTrackId = "75526548", Source = "Deezer", Title = "What's In Your Head", ArtistId = 10, AlbumId = 10, DurationSeconds = 330, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 148, ExternalTrackId = "75526549", Source = "Deezer", Title = "Tenderly", ArtistId = 10, AlbumId = 10, DurationSeconds = 304, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 149, ExternalTrackId = "75526550", Source = "Deezer", Title = "Running (Disclosure Remix)", ArtistId = 10, AlbumId = 10, DurationSeconds = 331, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 150, ExternalTrackId = "75526551", Source = "Deezer", Title = "Apollo", ArtistId = 10, AlbumId = 10, DurationSeconds = 403, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 151, ExternalTrackId = "75526552", Source = "Deezer", Title = "Boiling (Dixon Rework)", ArtistId = 10, AlbumId = 10, DurationSeconds = 571, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 152, ExternalTrackId = "75526553", Source = "Deezer", Title = "Boiling (Medlar Remix)", ArtistId = 10, AlbumId = 10, DurationSeconds = 352, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 153, ExternalTrackId = "75526554", Source = "Deezer", Title = "Control (Joe Goddard Remix)", ArtistId = 10, AlbumId = 10, DurationSeconds = 238, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 154, ExternalTrackId = "75526555", Source = "Deezer", Title = "F For You (TEED Remix)", ArtistId = 10, AlbumId = 10, DurationSeconds = 355, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 155, ExternalTrackId = "75526556", Source = "Deezer", Title = "Help Me Lose My Mind (Extended)", ArtistId = 10, AlbumId = 10, DurationSeconds = 428, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 156, ExternalTrackId = "75526557", Source = "Deezer", Title = "Help Me Lose My Mind (Live)", ArtistId = 10, AlbumId = 10, DurationSeconds = 518, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e44468007c45f2523d056a0b19eed80a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 1, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 2), CreatedAt = new DateTime(2020, 1, 1) },

            new Song { Id = 157, ExternalTrackId = "831216", Source = "Deezer", Title = "Wanna Be Startin' Somethin'", ArtistId = 11, AlbumId = 11, DurationSeconds = 363, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 158, ExternalTrackId = "831289", Source = "Deezer", Title = "Baby Be Mine", ArtistId = 11, AlbumId = 11, DurationSeconds = 260, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 159, ExternalTrackId = "831298", Source = "Deezer", Title = "The Girl Is Mine (with Paul McCartney)", ArtistId = 11, AlbumId = 11, DurationSeconds = 222, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 160, ExternalTrackId = "831319", Source = "Deezer", Title = "Thriller", ArtistId = 11, AlbumId = 11, DurationSeconds = 358, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 161, ExternalTrackId = "831272", Source = "Deezer", Title = "Beat It", ArtistId = 11, AlbumId = 11, DurationSeconds = 258, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 162, ExternalTrackId = "831313", Source = "Deezer", Title = "Billie Jean", ArtistId = 11, AlbumId = 11, DurationSeconds = 293, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 163, ExternalTrackId = "831196", Source = "Deezer", Title = "Human Nature", ArtistId = 11, AlbumId = 11, DurationSeconds = 245, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 164, ExternalTrackId = "831206", Source = "Deezer", Title = "P.Y.T. (Pretty Young Thing)", ArtistId = 11, AlbumId = 11, DurationSeconds = 239, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 165, ExternalTrackId = "831190", Source = "Deezer", Title = "The Lady In My Life", ArtistId = 11, AlbumId = 11, DurationSeconds = 297, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/544862aa5be45bc82ad4ab1a14daf63a/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1983, 8, 1), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 2), CreatedAt = new DateTime(2020, 8, 1) },

            new Song { Id = 166, ExternalTrackId = "59509421", Source = "Deezer", Title = "Bad (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 247, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 167, ExternalTrackId = "59509431", Source = "Deezer", Title = "The Way You Make Me Feel (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 298, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 168, ExternalTrackId = "59509441", Source = "Deezer", Title = "Speed Demon (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 242, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 169, ExternalTrackId = "59509451", Source = "Deezer", Title = "Liberian Girl (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 232, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 170, ExternalTrackId = "59509461", Source = "Deezer", Title = "Just Good Friends (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 246, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 171, ExternalTrackId = "59509471", Source = "Deezer", Title = "Another Part of Me (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 234, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 172, ExternalTrackId = "59509481", Source = "Deezer", Title = "Man in the Mirror (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 318, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 173, ExternalTrackId = "59509491", Source = "Deezer", Title = "I Just Can't Stop Loving You (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 251, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 174, ExternalTrackId = "59509501", Source = "Deezer", Title = "Dirty Diana (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 280, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 175, ExternalTrackId = "59509511", Source = "Deezer", Title = "Smooth Criminal (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 257, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 176, ExternalTrackId = "59509521", Source = "Deezer", Title = "Leave Me Alone (2012 Remaster)", ArtistId = 11, AlbumId = 12, DurationSeconds = 280, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/cad261eafd0c6c15811200d5039b5b50/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 9, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 9, 18), CreatedAt = new DateTime(2020, 9, 17) },

            new Song { Id = 177, ExternalTrackId = "8086126", Source = "Deezer", Title = "Rolling in the Deep", ArtistId = 12, AlbumId = 13, DurationSeconds = 228, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 178, ExternalTrackId = "8086127", Source = "Deezer", Title = "Rumour Has It", ArtistId = 12, AlbumId = 13, DurationSeconds = 223, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 179, ExternalTrackId = "8086128", Source = "Deezer", Title = "Turning Tables", ArtistId = 12, AlbumId = 13, DurationSeconds = 250, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 180, ExternalTrackId = "8086129", Source = "Deezer", Title = "Don't You Remember", ArtistId = 12, AlbumId = 13, DurationSeconds = 243, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 181, ExternalTrackId = "8086130", Source = "Deezer", Title = "Set Fire to the Rain", ArtistId = 12, AlbumId = 13, DurationSeconds = 242, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 182, ExternalTrackId = "8086131", Source = "Deezer", Title = "He Won't Go", ArtistId = 12, AlbumId = 13, DurationSeconds = 278, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 183, ExternalTrackId = "8086132", Source = "Deezer", Title = "Take It All", ArtistId = 12, AlbumId = 13, DurationSeconds = 228, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 184, ExternalTrackId = "8086133", Source = "Deezer", Title = "I'll Be Waiting", ArtistId = 12, AlbumId = 13, DurationSeconds = 241, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 185, ExternalTrackId = "8086134", Source = "Deezer", Title = "One and Only", ArtistId = 12, AlbumId = 13, DurationSeconds = 348, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 186, ExternalTrackId = "8086135", Source = "Deezer", Title = "Lovesong", ArtistId = 12, AlbumId = 13, DurationSeconds = 316, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 187, ExternalTrackId = "8086136", Source = "Deezer", Title = "Someone Like You", ArtistId = 12, AlbumId = 13, DurationSeconds = 285, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/dc1ce848d830ecc93521be5a78350364/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2011, 2, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 23), CreatedAt = new DateTime(2020, 2, 22) },

            new Song { Id = 188, ExternalTrackId = "629899752", Source = "Deezer", Title = "imagine", ArtistId = 13, AlbumId = 14, DurationSeconds = 212, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 189, ExternalTrackId = "629899762", Source = "Deezer", Title = "needy", ArtistId = 13, AlbumId = 14, DurationSeconds = 171, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 190, ExternalTrackId = "629899772", Source = "Deezer", Title = "NASA", ArtistId = 13, AlbumId = 14, DurationSeconds = 182, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 191, ExternalTrackId = "629899782", Source = "Deezer", Title = "bloodline", ArtistId = 13, AlbumId = 14, DurationSeconds = 215, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 192, ExternalTrackId = "629899792", Source = "Deezer", Title = "fake smile", ArtistId = 13, AlbumId = 14, DurationSeconds = 208, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 193, ExternalTrackId = "629899802", Source = "Deezer", Title = "bad idea", ArtistId = 13, AlbumId = 14, DurationSeconds = 266, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 194, ExternalTrackId = "629899812", Source = "Deezer", Title = "make up", ArtistId = 13, AlbumId = 14, DurationSeconds = 140, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 195, ExternalTrackId = "629899822", Source = "Deezer", Title = "ghostin", ArtistId = 13, AlbumId = 14, DurationSeconds = 270, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 196, ExternalTrackId = "629899832", Source = "Deezer", Title = "in my head", ArtistId = 13, AlbumId = 14, DurationSeconds = 222, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 197, ExternalTrackId = "629899842", Source = "Deezer", Title = "7 rings", ArtistId = 13, AlbumId = 14, DurationSeconds = 178, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 198, ExternalTrackId = "629899852", Source = "Deezer", Title = "thank u, next", ArtistId = 13, AlbumId = 14, DurationSeconds = 206, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 199, ExternalTrackId = "629899862", Source = "Deezer", Title = "break up with your girlfriend, i'm bored", ArtistId = 13, AlbumId = 14, DurationSeconds = 189, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49e86e935da829b44cb5ffae16826e55/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 2, 8), IsActive = true, LastSyncedAt = new DateTime(2020, 2, 9), CreatedAt = new DateTime(2020, 2, 8) },

            new Song { Id = 200, ExternalTrackId = "62376283", Source = "Deezer", Title = "Fresh Out The Runway", ArtistId = 14, AlbumId = 15, DurationSeconds = 224, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 201, ExternalTrackId = "62376284", Source = "Deezer", Title = "Diamonds", ArtistId = 14, AlbumId = 15, DurationSeconds = 225, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 202, ExternalTrackId = "62376285", Source = "Deezer", Title = "Numb", ArtistId = 14, AlbumId = 15, DurationSeconds = 205, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 203, ExternalTrackId = "62376286", Source = "Deezer", Title = "Pour It Up", ArtistId = 14, AlbumId = 15, DurationSeconds = 161, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 204, ExternalTrackId = "62376287", Source = "Deezer", Title = "Loveeeeeee Song", ArtistId = 14, AlbumId = 15, DurationSeconds = 256, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 205, ExternalTrackId = "62376288", Source = "Deezer", Title = "Jump", ArtistId = 14, AlbumId = 15, DurationSeconds = 264, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 206, ExternalTrackId = "62376289", Source = "Deezer", Title = "Right Now", ArtistId = 14, AlbumId = 15, DurationSeconds = 182, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 207, ExternalTrackId = "62376290", Source = "Deezer", Title = "What Now", ArtistId = 14, AlbumId = 15, DurationSeconds = 243, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 208, ExternalTrackId = "62376291", Source = "Deezer", Title = "Stay", ArtistId = 14, AlbumId = 15, DurationSeconds = 241, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 209, ExternalTrackId = "62376292", Source = "Deezer", Title = "Nobody's Business", ArtistId = 14, AlbumId = 15, DurationSeconds = 216, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 210, ExternalTrackId = "62376293", Source = "Deezer", Title = "Love Without Tragedy / Mother Mary", ArtistId = 14, AlbumId = 15, DurationSeconds = 418, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 211, ExternalTrackId = "62376294", Source = "Deezer", Title = "Get It Over With", ArtistId = 14, AlbumId = 15, DurationSeconds = 211, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 212, ExternalTrackId = "62376295", Source = "Deezer", Title = "No Love Allowed", ArtistId = 14, AlbumId = 15, DurationSeconds = 249, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 213, ExternalTrackId = "62376296", Source = "Deezer", Title = "Lost In Paradise", ArtistId = 14, AlbumId = 15, DurationSeconds = 216, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 214, ExternalTrackId = "62376297", Source = "Deezer", Title = "Half Of Me", ArtistId = 14, AlbumId = 15, DurationSeconds = 192, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 215, ExternalTrackId = "62376298", Source = "Deezer", Title = "Diamonds (Dave Aude 100 Extended)", ArtistId = 14, AlbumId = 15, DurationSeconds = 302, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 216, ExternalTrackId = "62376299", Source = "Deezer", Title = "Diamonds (Gregor Salto Downtempo Remix)", ArtistId = 14, AlbumId = 15, DurationSeconds = 269, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/246fb21e05da3d1460eb147123bb906b/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2012, 11, 19), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 20), CreatedAt = new DateTime(2020, 11, 19) },

            new Song { Id = 217, ExternalTrackId = "13529559", Source = "Deezer", Title = "S&M", ArtistId = 14, AlbumId = 16, DurationSeconds = 243, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 218, ExternalTrackId = "13529560", Source = "Deezer", Title = "What's My Name? (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 264, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 219, ExternalTrackId = "13529561", Source = "Deezer", Title = "Cheers (Drink To That) (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 261, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 220, ExternalTrackId = "13529562", Source = "Deezer", Title = "Fading (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 207, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 221, ExternalTrackId = "13529563", Source = "Deezer", Title = "Only Girl (In The World)", ArtistId = 14, AlbumId = 16, DurationSeconds = 235, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 222, ExternalTrackId = "13529564", Source = "Deezer", Title = "California King Bed (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 251, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 223, ExternalTrackId = "13529565", Source = "Deezer", Title = "Man Down (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 267, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 224, ExternalTrackId = "13529566", Source = "Deezer", Title = "Raining Men (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 224, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 225, ExternalTrackId = "13529567", Source = "Deezer", Title = "Complicated (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 257, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 226, ExternalTrackId = "13529568", Source = "Deezer", Title = "Skin (Album Version)", ArtistId = 14, AlbumId = 16, DurationSeconds = 303, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 227, ExternalTrackId = "13529569", Source = "Deezer", Title = "Love The Way You Lie (Part II)", ArtistId = 14, AlbumId = 16, DurationSeconds = 296, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/3a12c64bb52a167944783878ffe41f02/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 11, 16), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 17), CreatedAt = new DateTime(2020, 11, 16) },

            new Song { Id = 228, ExternalTrackId = "126772729", Source = "Deezer", Title = "Ultralight Beam", ArtistId = 4, AlbumId = 17, DurationSeconds = 320, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 229, ExternalTrackId = "126772731", Source = "Deezer", Title = "Father Stretch My Hands Pt. 1", ArtistId = 4, AlbumId = 17, DurationSeconds = 135, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 230, ExternalTrackId = "126772733", Source = "Deezer", Title = "Pt. 2", ArtistId = 4, AlbumId = 17, DurationSeconds = 130, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 231, ExternalTrackId = "126772735", Source = "Deezer", Title = "Famous", ArtistId = 4, AlbumId = 17, DurationSeconds = 192, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 232, ExternalTrackId = "126772737", Source = "Deezer", Title = "Feedback", ArtistId = 4, AlbumId = 17, DurationSeconds = 147, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 233, ExternalTrackId = "126772739", Source = "Deezer", Title = "Low Lights", ArtistId = 4, AlbumId = 17, DurationSeconds = 131, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 234, ExternalTrackId = "126772741", Source = "Deezer", Title = "Highlights", ArtistId = 4, AlbumId = 17, DurationSeconds = 199, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 235, ExternalTrackId = "126772743", Source = "Deezer", Title = "Freestyle 4", ArtistId = 4, AlbumId = 17, DurationSeconds = 123, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 236, ExternalTrackId = "126772745", Source = "Deezer", Title = "I Love Kanye", ArtistId = 4, AlbumId = 17, DurationSeconds = 44, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 237, ExternalTrackId = "126772747", Source = "Deezer", Title = "Waves", ArtistId = 4, AlbumId = 17, DurationSeconds = 181, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 238, ExternalTrackId = "126772749", Source = "Deezer", Title = "FML", ArtistId = 4, AlbumId = 17, DurationSeconds = 236, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 239, ExternalTrackId = "126772751", Source = "Deezer", Title = "Real Friends", ArtistId = 4, AlbumId = 17, DurationSeconds = 251, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 240, ExternalTrackId = "126772753", Source = "Deezer", Title = "Wolves", ArtistId = 4, AlbumId = 17, DurationSeconds = 301, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 241, ExternalTrackId = "126772755", Source = "Deezer", Title = "Frank's Track", ArtistId = 4, AlbumId = 17, DurationSeconds = 38, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 242, ExternalTrackId = "126772757", Source = "Deezer", Title = "Siiiiiiiiilver Surffffeeeeer Intermission", ArtistId = 4, AlbumId = 17, DurationSeconds = 56, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 243, ExternalTrackId = "126772759", Source = "Deezer", Title = "30 Hours", ArtistId = 4, AlbumId = 17, DurationSeconds = 323, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 244, ExternalTrackId = "126772761", Source = "Deezer", Title = "No More Parties In LA", ArtistId = 4, AlbumId = 17, DurationSeconds = 374, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 245, ExternalTrackId = "126772763", Source = "Deezer", Title = "Facts (Charlie Heat Version)", ArtistId = 4, AlbumId = 17, DurationSeconds = 200, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 246, ExternalTrackId = "126772765", Source = "Deezer", Title = "Fade", ArtistId = 4, AlbumId = 17, DurationSeconds = 193, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 247, ExternalTrackId = "126772767", Source = "Deezer", Title = "Saint Pablo", ArtistId = 4, AlbumId = 17, DurationSeconds = 372, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/e055ecc8d01680cda0460017087728be/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2016, 6, 15), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 16), CreatedAt = new DateTime(2020, 6, 15) },

            new Song { Id = 248, ExternalTrackId = "725929", Source = "Deezer", Title = "By the Way", ArtistId = 15, AlbumId = 18, DurationSeconds = 216, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 249, ExternalTrackId = "725937", Source = "Deezer", Title = "Universally Speaking", ArtistId = 15, AlbumId = 18, DurationSeconds = 256, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 250, ExternalTrackId = "725943", Source = "Deezer", Title = "This Is the Place", ArtistId = 15, AlbumId = 18, DurationSeconds = 257, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 251, ExternalTrackId = "725950", Source = "Deezer", Title = "Dosed", ArtistId = 15, AlbumId = 18, DurationSeconds = 311, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 252, ExternalTrackId = "725959", Source = "Deezer", Title = "Don't Forget Me", ArtistId = 15, AlbumId = 18, DurationSeconds = 277, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 253, ExternalTrackId = "725965", Source = "Deezer", Title = "The Zephyr Song", ArtistId = 15, AlbumId = 18, DurationSeconds = 231, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 254, ExternalTrackId = "725971", Source = "Deezer", Title = "Can't Stop", ArtistId = 15, AlbumId = 18, DurationSeconds = 269, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 255, ExternalTrackId = "725976", Source = "Deezer", Title = "I Could Die for You", ArtistId = 15, AlbumId = 18, DurationSeconds = 192, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 256, ExternalTrackId = "725984", Source = "Deezer", Title = "Midnight", ArtistId = 15, AlbumId = 18, DurationSeconds = 295, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 257, ExternalTrackId = "725989", Source = "Deezer", Title = "Throw Away Your Television", ArtistId = 15, AlbumId = 18, DurationSeconds = 224, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 258, ExternalTrackId = "725995", Source = "Deezer", Title = "Cabron", ArtistId = 15, AlbumId = 18, DurationSeconds = 218, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 259, ExternalTrackId = "726003", Source = "Deezer", Title = "Tear", ArtistId = 15, AlbumId = 18, DurationSeconds = 317, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 260, ExternalTrackId = "726007", Source = "Deezer", Title = "On Mercury", ArtistId = 15, AlbumId = 18, DurationSeconds = 207, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 261, ExternalTrackId = "726014", Source = "Deezer", Title = "Minor Thing", ArtistId = 15, AlbumId = 18, DurationSeconds = 217, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 262, ExternalTrackId = "726021", Source = "Deezer", Title = "Warm Tape", ArtistId = 15, AlbumId = 18, DurationSeconds = 255, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 263, ExternalTrackId = "726027", Source = "Deezer", Title = "Venice Queen", ArtistId = 15, AlbumId = 18, DurationSeconds = 367, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 264, ExternalTrackId = "726034", Source = "Deezer", Title = "Runaway (2006 Remaster)", ArtistId = 15, AlbumId = 18, DurationSeconds = 270, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 265, ExternalTrackId = "726042", Source = "Deezer", Title = "Bicycle Song (2006 Remaster)", ArtistId = 15, AlbumId = 18, DurationSeconds = 203, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/49b073f55550d41055e02c493f9a7d39/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2002, 6, 25), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 26), CreatedAt = new DateTime(2020, 6, 25) },

            new Song { Id = 266, ExternalTrackId = "680516", Source = "Deezer", Title = "Dani California", ArtistId = 15, AlbumId = 19, DurationSeconds = 282, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 267, ExternalTrackId = "680518", Source = "Deezer", Title = "Snow (Hey Oh)", ArtistId = 15, AlbumId = 19, DurationSeconds = 334, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 268, ExternalTrackId = "680520", Source = "Deezer", Title = "Charlie", ArtistId = 15, AlbumId = 19, DurationSeconds = 277, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 269, ExternalTrackId = "680522", Source = "Deezer", Title = "Stadium Arcadium", ArtistId = 15, AlbumId = 19, DurationSeconds = 314, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 270, ExternalTrackId = "680524", Source = "Deezer", Title = "Hump de Bump", ArtistId = 15, AlbumId = 19, DurationSeconds = 213, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 271, ExternalTrackId = "680525", Source = "Deezer", Title = "She's Only 18", ArtistId = 15, AlbumId = 19, DurationSeconds = 205, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 272, ExternalTrackId = "680528", Source = "Deezer", Title = "Slow Cheetah", ArtistId = 15, AlbumId = 19, DurationSeconds = 319, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 273, ExternalTrackId = "680529", Source = "Deezer", Title = "Torture Me", ArtistId = 15, AlbumId = 19, DurationSeconds = 224, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 274, ExternalTrackId = "680532", Source = "Deezer", Title = "Strip My Mind", ArtistId = 15, AlbumId = 19, DurationSeconds = 259, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 275, ExternalTrackId = "680534", Source = "Deezer", Title = "Especially in Michigan", ArtistId = 15, AlbumId = 19, DurationSeconds = 240, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 276, ExternalTrackId = "680536", Source = "Deezer", Title = "Warlocks", ArtistId = 15, AlbumId = 19, DurationSeconds = 205, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 277, ExternalTrackId = "680537", Source = "Deezer", Title = "C'mon Girl", ArtistId = 15, AlbumId = 19, DurationSeconds = 228, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 278, ExternalTrackId = "680538", Source = "Deezer", Title = "Wet Sand", ArtistId = 15, AlbumId = 19, DurationSeconds = 309, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 279, ExternalTrackId = "680539", Source = "Deezer", Title = "Hey", ArtistId = 15, AlbumId = 19, DurationSeconds = 339, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 280, ExternalTrackId = "680540", Source = "Deezer", Title = "Desecration Smile", ArtistId = 15, AlbumId = 19, DurationSeconds = 301, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 281, ExternalTrackId = "680542", Source = "Deezer", Title = "Tell Me Baby", ArtistId = 15, AlbumId = 19, DurationSeconds = 247, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 282, ExternalTrackId = "680544", Source = "Deezer", Title = "Hard to Concentrate", ArtistId = 15, AlbumId = 19, DurationSeconds = 241, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 283, ExternalTrackId = "680546", Source = "Deezer", Title = "21st Century", ArtistId = 15, AlbumId = 19, DurationSeconds = 262, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 284, ExternalTrackId = "680548", Source = "Deezer", Title = "She Looks to Me", ArtistId = 15, AlbumId = 19, DurationSeconds = 245, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 285, ExternalTrackId = "680550", Source = "Deezer", Title = "Readymade", ArtistId = 15, AlbumId = 19, DurationSeconds = 270, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 286, ExternalTrackId = "680552", Source = "Deezer", Title = "If", ArtistId = 15, AlbumId = 19, DurationSeconds = 172, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 287, ExternalTrackId = "680555", Source = "Deezer", Title = "Make You Feel Better", ArtistId = 15, AlbumId = 19, DurationSeconds = 231, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 288, ExternalTrackId = "680557", Source = "Deezer", Title = "Animal Bar", ArtistId = 15, AlbumId = 19, DurationSeconds = 325, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 289, ExternalTrackId = "680559", Source = "Deezer", Title = "So Much I", ArtistId = 15, AlbumId = 19, DurationSeconds = 224, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 290, ExternalTrackId = "680560", Source = "Deezer", Title = "Storm in a Teacup", ArtistId = 15, AlbumId = 19, DurationSeconds = 224, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/a3a4028b1192c50d82a579439cbfc4af/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2006, 5, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 6), CreatedAt = new DateTime(2020, 5, 5) },

            new Song { Id = 291, ExternalTrackId = "138539971", Source = "Deezer", Title = "Airbag", ArtistId = 16, AlbumId = 20, DurationSeconds = 287, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 292, ExternalTrackId = "138539973", Source = "Deezer", Title = "Paranoid Android", ArtistId = 16, AlbumId = 20, DurationSeconds = 387, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 293, ExternalTrackId = "138539975", Source = "Deezer", Title = "Subterranean Homesick Alien", ArtistId = 16, AlbumId = 20, DurationSeconds = 267, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 294, ExternalTrackId = "138539977", Source = "Deezer", Title = "Exit Music (For A Film)", ArtistId = 16, AlbumId = 20, DurationSeconds = 267, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 295, ExternalTrackId = "138539979", Source = "Deezer", Title = "Let Down", ArtistId = 16, AlbumId = 20, DurationSeconds = 299, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 296, ExternalTrackId = "138539981", Source = "Deezer", Title = "Karma Police", ArtistId = 16, AlbumId = 20, DurationSeconds = 264, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 297, ExternalTrackId = "138539983", Source = "Deezer", Title = "Fitter Happier", ArtistId = 16, AlbumId = 20, DurationSeconds = 117, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 298, ExternalTrackId = "138539985", Source = "Deezer", Title = "Electioneering", ArtistId = 16, AlbumId = 20, DurationSeconds = 230, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 299, ExternalTrackId = "138539987", Source = "Deezer", Title = "Climbing Up the Walls", ArtistId = 16, AlbumId = 20, DurationSeconds = 285, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 300, ExternalTrackId = "138539989", Source = "Deezer", Title = "No Surprises", ArtistId = 16, AlbumId = 20, DurationSeconds = 229, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 301, ExternalTrackId = "138539991", Source = "Deezer", Title = "Lucky", ArtistId = 16, AlbumId = 20, DurationSeconds = 258, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 302, ExternalTrackId = "138539993", Source = "Deezer", Title = "The Tourist", ArtistId = 16, AlbumId = 20, DurationSeconds = 326, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/05a186e0a859a36f9cd51cdae2158fe1/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1997, 6, 17), IsActive = true, LastSyncedAt = new DateTime(2020, 6, 18), CreatedAt = new DateTime(2020, 6, 17) },

            new Song { Id = 303, ExternalTrackId = "81836818", Source = "Deezer", Title = "Divinity", ArtistId = 17, AlbumId = 21, DurationSeconds = 367, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 304, ExternalTrackId = "81836820", Source = "Deezer", Title = "Sad Machine", ArtistId = 17, AlbumId = 21, DurationSeconds = 350, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 305, ExternalTrackId = "81836822", Source = "Deezer", Title = "Years Of War", ArtistId = 17, AlbumId = 21, DurationSeconds = 233, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 306, ExternalTrackId = "81836824", Source = "Deezer", Title = "Flicker", ArtistId = 17, AlbumId = 21, DurationSeconds = 277, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 307, ExternalTrackId = "81836826", Source = "Deezer", Title = "Fresh Static Snow", ArtistId = 17, AlbumId = 21, DurationSeconds = 359, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 308, ExternalTrackId = "81836828", Source = "Deezer", Title = "Polygon Dust", ArtistId = 17, AlbumId = 21, DurationSeconds = 208, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 309, ExternalTrackId = "81836830", Source = "Deezer", Title = "Hear The Bells", ArtistId = 17, AlbumId = 21, DurationSeconds = 285, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf6f1044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 310, ExternalTrackId = "81836832", Source = "Deezer", Title = "Natural Light", ArtistId = 17, AlbumId = 21, DurationSeconds = 141, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf61044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 311, ExternalTrackId = "81836834", Source = "Deezer", Title = "Unfold", ArtistId = 17, AlbumId = 21, DurationSeconds = 264, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf61044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 312, ExternalTrackId = "81836836", Source = "Deezer", Title = "Sea Of Voices", ArtistId = 17, AlbumId = 21, DurationSeconds = 298, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf61044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 313, ExternalTrackId = "81836838", Source = "Deezer", Title = "Fellow Feeling", ArtistId = 17, AlbumId = 21, DurationSeconds = 349, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf61044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 314, ExternalTrackId = "81836840", Source = "Deezer", Title = "Goodbye To A World", ArtistId = 17, AlbumId = 21, DurationSeconds = 328, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b1cf61044ae1e051824e34099688347/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2014, 8, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 8, 14), CreatedAt = new DateTime(2020, 8, 13) },

            new Song { Id = 315, ExternalTrackId = "830336912", Source = "Deezer", Title = "Golden", ArtistId = 18, AlbumId = 22, DurationSeconds = 208, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 316, ExternalTrackId = "830336922", Source = "Deezer", Title = "Watermelon Sugar", ArtistId = 18, AlbumId = 22, DurationSeconds = 173, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 317, ExternalTrackId = "830336932", Source = "Deezer", Title = "Adore You", ArtistId = 18, AlbumId = 22, DurationSeconds = 207, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 318, ExternalTrackId = "830336942", Source = "Deezer", Title = "Lights Up", ArtistId = 18, AlbumId = 22, DurationSeconds = 174, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 319, ExternalTrackId = "830336952", Source = "Deezer", Title = "Cherry", ArtistId = 18, AlbumId = 22, DurationSeconds = 259, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 320, ExternalTrackId = "830336962", Source = "Deezer", Title = "Falling", ArtistId = 18, AlbumId = 22, DurationSeconds = 240, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 321, ExternalTrackId = "830336972", Source = "Deezer", Title = "To Be So Lonely", ArtistId = 18, AlbumId = 22, DurationSeconds = 192, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 322, ExternalTrackId = "830336982", Source = "Deezer", Title = "She", ArtistId = 18, AlbumId = 22, DurationSeconds = 362, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 323, ExternalTrackId = "830336992", Source = "Deezer", Title = "Sunflower, Vol. 6", ArtistId = 18, AlbumId = 22, DurationSeconds = 221, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 324, ExternalTrackId = "830337002", Source = "Deezer", Title = "Canyon Moon", ArtistId = 18, AlbumId = 22, DurationSeconds = 189, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 325, ExternalTrackId = "830337012", Source = "Deezer", Title = "Treat People With Kindness", ArtistId = 18, AlbumId = 22, DurationSeconds = 197, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 326, ExternalTrackId = "830337022", Source = "Deezer", Title = "Fine Line", ArtistId = 18, AlbumId = 22, DurationSeconds = 377, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/346c524c15ecccbc4a8a78e8972a352c/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2019, 12, 13), IsActive = true, LastSyncedAt = new DateTime(2020, 12, 14), CreatedAt = new DateTime(2020, 12, 13) },

            new Song { Id = 327, ExternalTrackId = "4677472", Source = "Deezer", Title = "Curtains Up", ArtistId = 19, AlbumId = 23, DurationSeconds = 47, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 328, ExternalTrackId = "4677473", Source = "Deezer", Title = "Evil Deeds", ArtistId = 19, AlbumId = 23, DurationSeconds = 260, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 329, ExternalTrackId = "4677474", Source = "Deezer", Title = "Never Enough", ArtistId = 19, AlbumId = 23, DurationSeconds = 160, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 330, ExternalTrackId = "4677475", Source = "Deezer", Title = "Yellow Brick Road", ArtistId = 19, AlbumId = 23, DurationSeconds = 346, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 331, ExternalTrackId = "4677476", Source = "Deezer", Title = "Like Toy Soldiers", ArtistId = 19, AlbumId = 23, DurationSeconds = 297, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 332, ExternalTrackId = "4677477", Source = "Deezer", Title = "Mosh", ArtistId = 19, AlbumId = 23, DurationSeconds = 318, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 333, ExternalTrackId = "4677478", Source = "Deezer", Title = "Puke", ArtistId = 19, AlbumId = 23, DurationSeconds = 248, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 334, ExternalTrackId = "4677479", Source = "Deezer", Title = "My 1st Single", ArtistId = 19, AlbumId = 23, DurationSeconds = 303, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 335, ExternalTrackId = "4677480", Source = "Deezer", Title = "Paul (Skit)", ArtistId = 19, AlbumId = 23, DurationSeconds = 32, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 336, ExternalTrackId = "4677481", Source = "Deezer", Title = "Rain Man", ArtistId = 19, AlbumId = 23, DurationSeconds = 314, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 337, ExternalTrackId = "4677482", Source = "Deezer", Title = "Big Weenie", ArtistId = 19, AlbumId = 23, DurationSeconds = 267, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 338, ExternalTrackId = "4677483", Source = "Deezer", Title = "Em Calls Paul (Skit)", ArtistId = 19, AlbumId = 23, DurationSeconds = 72, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 339, ExternalTrackId = "4677484", Source = "Deezer", Title = "Just Lose It", ArtistId = 19, AlbumId = 23, DurationSeconds = 249, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 340, ExternalTrackId = "4677485", Source = "Deezer", Title = "Ass Like That", ArtistId = 19, AlbumId = 23, DurationSeconds = 265, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 341, ExternalTrackId = "4677486", Source = "Deezer", Title = "Spend Some Time", ArtistId = 19, AlbumId = 23, DurationSeconds = 310, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 342, ExternalTrackId = "4677487", Source = "Deezer", Title = "Mockingbird", ArtistId = 19, AlbumId = 23, DurationSeconds = 251, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 343, ExternalTrackId = "4677488", Source = "Deezer", Title = "Crazy In Love", ArtistId = 19, AlbumId = 23, DurationSeconds = 242, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 344, ExternalTrackId = "4677489", Source = "Deezer", Title = "One Shot 2 Shot", ArtistId = 19, AlbumId = 23, DurationSeconds = 267, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 345, ExternalTrackId = "4677490", Source = "Deezer", Title = "Final Thought (Skit)", ArtistId = 19, AlbumId = 23, DurationSeconds = 30, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 346, ExternalTrackId = "4677491", Source = "Deezer", Title = "Encore / Curtains Down", ArtistId = 19, AlbumId = 23, DurationSeconds = 347, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 347, ExternalTrackId = "4677492", Source = "Deezer", Title = "We As Americans (Album Version)", ArtistId = 19, AlbumId = 23, DurationSeconds = 276, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 348, ExternalTrackId = "4677493", Source = "Deezer", Title = "Love You More (Album Version)", ArtistId = 19, AlbumId = 23, DurationSeconds = 284, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 349, ExternalTrackId = "4677494", Source = "Deezer", Title = "Ricky Ticky Toc (Album Version)", ArtistId = 19, AlbumId = 23, DurationSeconds = 172, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/c70c68f2f8aa376093774b931e04d018/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2004, 11, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 13), CreatedAt = new DateTime(2020, 11, 12) },

            new Song { Id = 350, ExternalTrackId = "15593559", Source = "Deezer", Title = "Jam", ArtistId = 11, AlbumId = 24, DurationSeconds = 339, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 351, ExternalTrackId = "15593560", Source = "Deezer", Title = "Why You Wanna Trip on Me", ArtistId = 11, AlbumId = 24, DurationSeconds = 325, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 352, ExternalTrackId = "15593561", Source = "Deezer", Title = "In the Closet", ArtistId = 11, AlbumId = 24, DurationSeconds = 392, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 353, ExternalTrackId = "15593562", Source = "Deezer", Title = "She Drives Me Wild", ArtistId = 11, AlbumId = 24, DurationSeconds = 221, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 354, ExternalTrackId = "15593563", Source = "Deezer", Title = "Remember the Time", ArtistId = 11, AlbumId = 24, DurationSeconds = 239, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 355, ExternalTrackId = "15593564", Source = "Deezer", Title = "Can't Let Her Get Away", ArtistId = 11, AlbumId = 24, DurationSeconds = 299, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 356, ExternalTrackId = "15593565", Source = "Deezer", Title = "Heal the World", ArtistId = 11, AlbumId = 24, DurationSeconds = 384, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 357, ExternalTrackId = "15593566", Source = "Deezer", Title = "Black or White", ArtistId = 11, AlbumId = 24, DurationSeconds = 256, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 358, ExternalTrackId = "15593567", Source = "Deezer", Title = "Who Is It", ArtistId = 11, AlbumId = 24, DurationSeconds = 395, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 359, ExternalTrackId = "15593568", Source = "Deezer", Title = "Give In to Me", ArtistId = 11, AlbumId = 24, DurationSeconds = 330, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 360, ExternalTrackId = "15593569", Source = "Deezer", Title = "Will You Be There", ArtistId = 11, AlbumId = 24, DurationSeconds = 460, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 361, ExternalTrackId = "15593570", Source = "Deezer", Title = "Keep the Faith", ArtistId = 11, AlbumId = 24, DurationSeconds = 357, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 362, ExternalTrackId = "15593571", Source = "Deezer", Title = "Gone Too Soon", ArtistId = 11, AlbumId = 24, DurationSeconds = 202, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 363, ExternalTrackId = "15593572", Source = "Deezer", Title = "Dangerous", ArtistId = 11, AlbumId = 24, DurationSeconds = 420, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/93a5354699d552666448e1c87c976605/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(1991, 11, 21), IsActive = true, LastSyncedAt = new DateTime(2020, 11, 22), CreatedAt = new DateTime(2020, 11, 21) },

            new Song { Id = 364, ExternalTrackId = "8011849", Source = "Deezer", Title = "Grenade", ArtistId = 20, AlbumId = 25, DurationSeconds = 222, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 365, ExternalTrackId = "8011850", Source = "Deezer", Title = "Just the Way You Are", ArtistId = 20, AlbumId = 25, DurationSeconds = 220, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 366, ExternalTrackId = "8011851", Source = "Deezer", Title = "Our First Time", ArtistId = 20, AlbumId = 25, DurationSeconds = 243, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 367, ExternalTrackId = "8011852", Source = "Deezer", Title = "Runaway Baby", ArtistId = 20, AlbumId = 25, DurationSeconds = 148, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 368, ExternalTrackId = "8011853", Source = "Deezer", Title = "The Lazy Song", ArtistId = 20, AlbumId = 25, DurationSeconds = 189, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 369, ExternalTrackId = "8011854", Source = "Deezer", Title = "Marry You", ArtistId = 20, AlbumId = 25, DurationSeconds = 230, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 370, ExternalTrackId = "8011855", Source = "Deezer", Title = "Talking to the Moon", ArtistId = 20, AlbumId = 25, DurationSeconds = 217, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 371, ExternalTrackId = "8011856", Source = "Deezer", Title = "Liquor Store Blues (feat. Damian Marley)", ArtistId = 20, AlbumId = 25, DurationSeconds = 229, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 372, ExternalTrackId = "8011857", Source = "Deezer", Title = "Count on Me", ArtistId = 20, AlbumId = 25, DurationSeconds = 197, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 373, ExternalTrackId = "8011858", Source = "Deezer", Title = "The Other Side (feat. CeeLo Green and B.o.B)", ArtistId = 20, AlbumId = 25, DurationSeconds = 228, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 374, ExternalTrackId = "8011859", Source = "Deezer", Title = "Somewhere in Brooklyn", ArtistId = 20, AlbumId = 25, DurationSeconds = 181, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 375, ExternalTrackId = "8011860", Source = "Deezer", Title = "Talking to the Moon (Acoustic Piano Version)", ArtistId = 20, AlbumId = 25, DurationSeconds = 217, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/5b59dc18e109515420f8237719bd2186/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2010, 10, 5), IsActive = true, LastSyncedAt = new DateTime(2020, 10, 6), CreatedAt = new DateTime(2020, 10, 5) },

            new Song { Id = 376, ExternalTrackId = "96816466", Source = "Deezer", Title = "Isometric (Intro)", ArtistId = 21, AlbumId = 26, DurationSeconds = 80, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 377, ExternalTrackId = "96816468", Source = "Deezer", Title = "You're On (feat. Kyan)", ArtistId = 21, AlbumId = 26, DurationSeconds = 192, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 378, ExternalTrackId = "96816470", Source = "Deezer", Title = "OK", ArtistId = 21, AlbumId = 26, DurationSeconds = 182, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 379, ExternalTrackId = "96816472", Source = "Deezer", Title = "La Lune (feat. Dan Smith)", ArtistId = 21, AlbumId = 26, DurationSeconds = 219, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 380, ExternalTrackId = "96816474", Source = "Deezer", Title = "Pay No Mind (feat. Passion Pit)", ArtistId = 21, AlbumId = 26, DurationSeconds = 249, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 381, ExternalTrackId = "96816476", Source = "Deezer", Title = "Beings", ArtistId = 21, AlbumId = 26, DurationSeconds = 215, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 382, ExternalTrackId = "96816478", Source = "Deezer", Title = "Imperium", ArtistId = 21, AlbumId = 26, DurationSeconds = 198, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 383, ExternalTrackId = "96816480", Source = "Deezer", Title = "Zephyr", ArtistId = 21, AlbumId = 26, DurationSeconds = 222, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 384, ExternalTrackId = "96816482", Source = "Deezer", Title = "Nonsense (feat. Mark Foster)", ArtistId = 21, AlbumId = 26, DurationSeconds = 224, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 385, ExternalTrackId = "96816484", Source = "Deezer", Title = "Innocence (feat. Aquilo)", ArtistId = 21, AlbumId = 26, DurationSeconds = 224, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 386, ExternalTrackId = "96816486", Source = "Deezer", Title = "Pixel Empire", ArtistId = 21, AlbumId = 26, DurationSeconds = 244, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 387, ExternalTrackId = "96816488", Source = "Deezer", Title = "Home", ArtistId = 21, AlbumId = 26, DurationSeconds = 225, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

        new Song { Id = 388, ExternalTrackId = "96816490", Source = "Deezer", Title = "Icarus", ArtistId = 21, AlbumId = 26, DurationSeconds = 214, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 389, ExternalTrackId = "96816492", Source = "Deezer", Title = "Finale (feat. Nicholas Petricca)", ArtistId = 21, AlbumId = 26, DurationSeconds = 205, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 390, ExternalTrackId = "96816494", Source = "Deezer", Title = "The City", ArtistId = 21, AlbumId = 26, DurationSeconds = 233, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 391, ExternalTrackId = "96816496", Source = "Deezer", Title = "Cut the Kid", ArtistId = 21, AlbumId = 26, DurationSeconds = 200, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 392, ExternalTrackId = "96816498", Source = "Deezer", Title = "Technicolor", ArtistId = 21, AlbumId = 26, DurationSeconds = 385, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 393, ExternalTrackId = "96816500", Source = "Deezer", Title = "Only Way Out (feat. Vancouver Sleep Clinic)", ArtistId = 21, AlbumId = 26, DurationSeconds = 226, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/42fc281a193f26af6d105fa495e2fdab/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2015, 3, 30), IsActive = true, LastSyncedAt = new DateTime(2020, 3, 31), CreatedAt = new DateTime(2020, 3, 30) },

            new Song { Id = 394, ExternalTrackId = "1756569567", Source = "Deezer", Title = "Music For a Sushi Restaurant", ArtistId = 18, AlbumId = 27, DurationSeconds = 193, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 395, ExternalTrackId = "1756569577", Source = "Deezer", Title = "Late Night Talking", ArtistId = 18, AlbumId = 27, DurationSeconds = 177, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 396, ExternalTrackId = "1756569587", Source = "Deezer", Title = "Grapejuice", ArtistId = 18, AlbumId = 27, DurationSeconds = 191, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 397, ExternalTrackId = "1756569597", Source = "Deezer", Title = "As It Was", ArtistId = 18, AlbumId = 27, DurationSeconds = 167, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 398, ExternalTrackId = "1756569607", Source = "Deezer", Title = "Daylight", ArtistId = 18, AlbumId = 27, DurationSeconds = 164, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 399, ExternalTrackId = "1756569617", Source = "Deezer", Title = "Little Freak", ArtistId = 18, AlbumId = 27, DurationSeconds = 202, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 400, ExternalTrackId = "1756569627", Source = "Deezer", Title = "Matilda", ArtistId = 18, AlbumId = 27, DurationSeconds = 245, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 401, ExternalTrackId = "1756569637", Source = "Deezer", Title = "Cinema", ArtistId = 18, AlbumId = 27, DurationSeconds = 243, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 402, ExternalTrackId = "1756569647", Source = "Deezer", Title = "Daydreaming", ArtistId = 18, AlbumId = 27, DurationSeconds = 187, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 403, ExternalTrackId = "1756569657", Source = "Deezer", Title = "Keep Driving", ArtistId = 18, AlbumId = 27, DurationSeconds = 140, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 404, ExternalTrackId = "1756569667", Source = "Deezer", Title = "Satellite", ArtistId = 18, AlbumId = 27, DurationSeconds = 218, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 405, ExternalTrackId = "1756569677", Source = "Deezer", Title = "Boyfriends", ArtistId = 18, AlbumId = 27, DurationSeconds = 194, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 406, ExternalTrackId = "1756569687", Source = "Deezer", Title = "Love Of My Life", ArtistId = 18, AlbumId = 27, DurationSeconds = 191, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2022, 5, 20), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 21), CreatedAt = new DateTime(2020, 5, 20) },

            new Song { Id = 407, ExternalTrackId = "360301941", Source = "Deezer", Title = "Meet Me in the Hallway", ArtistId = 18, AlbumId = 28, DurationSeconds = 228, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 408, ExternalTrackId = "360301951", Source = "Deezer", Title = "Sign of the Times", ArtistId = 18, AlbumId = 28, DurationSeconds = 340, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 409, ExternalTrackId = "360301961", Source = "Deezer", Title = "Carolina", ArtistId = 18, AlbumId = 28, DurationSeconds = 189, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 410, ExternalTrackId = "360301971", Source = "Deezer", Title = "Two Ghosts", ArtistId = 18, AlbumId = 28, DurationSeconds = 229, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 411, ExternalTrackId = "360301981", Source = "Deezer", Title = "Sweet Creature", ArtistId = 18, AlbumId = 28, DurationSeconds = 224, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 412, ExternalTrackId = "360301991", Source = "Deezer", Title = "Only Angel", ArtistId = 18, AlbumId = 28, DurationSeconds = 291, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 413, ExternalTrackId = "360302001", Source = "Deezer", Title = "Kiwi", ArtistId = 18, AlbumId = 28, DurationSeconds = 176, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 414, ExternalTrackId = "360302011", Source = "Deezer", Title = "Ever Since New York", ArtistId = 18, AlbumId = 28, DurationSeconds = 253, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 415, ExternalTrackId = "360302021", Source = "Deezer", Title = "Woman", ArtistId = 18, AlbumId = 28, DurationSeconds = 278, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 416, ExternalTrackId = "360302031", Source = "Deezer", Title = "From the Dining Table", ArtistId = 18, AlbumId = 28, DurationSeconds = 211, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/b0e936124f59e669ddba02ebe5893f95/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2017, 5, 12), IsActive = true, LastSyncedAt = new DateTime(2020, 5, 13), CreatedAt = new DateTime(2020, 5, 12) },

            new Song { Id = 417, ExternalTrackId = "3791401032", Source = "Deezer", Title = "Aperture", ArtistId = 18, AlbumId = 29, DurationSeconds = 311, PreviewUrl = null, CoverUrl = "https://cdn-images.dzcdn.net/images/cover/fee004942feff253f7bbca63740ab519/250x250-000000-80-0-0.jpg", ReleaseDate = new DateTime(2026, 1, 22), IsActive = true, LastSyncedAt = new DateTime(2020, 1, 23), CreatedAt = new DateTime(2020, 1, 22) }

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


            var additionalUsers = GenerateAdditionalUsers(userHash, userSalt, 250);
            var allUsers = baseUsers.Concat(additionalUsers).ToList();
            var playHistories = GeneratePlayHistories(allUsers.Count, 417, 2000);

            modelBuilder.Entity<User>().HasData(allUsers);

            modelBuilder.Entity<UserRole>().HasData(GenerateUserRoles(260));

            modelBuilder.Entity<PlayHistory>().HasData(playHistories);

            modelBuilder.Entity<SubscriptionPlan>().HasData(
                new SubscriptionPlan
                {
                    Id = 1,
                    Name = "Basic account",
                    Price = 0,
                    DurationDays = 0,
                    Description = "Osnovni plan sa preview pristupom",
                    IsActive = true
                },
                new SubscriptionPlan
                {
                    Id = 2,
                    Name = "Premium account",
                    Price = 4.99f,
                    DurationDays = 30,
                    Description = "Premium plan za 30 dana",
                    IsActive = true
                }
            );

            modelBuilder.Entity<Question>().HasData(

    new Question
    {
        Id = 1,
        UserId = 2,
        Title = "Kako dodati pjesmu u playlistu?",
        Content = "Ne mogu pronaći opciju za dodavanje pjesme u playlistu.",
        Status = "Answered",
        Answer = "Klikni na tri tačke pored pjesme i izaberi 'Add to playlist'.",
        CreatedAt = new DateTime(2026, 3, 20, 10, 15, 0),
        AnsweredAt = new DateTime(2026, 3, 20, 11, 0, 0)
    },

    new Question
    {
        Id = 2,
        UserId = 3,
        Title = "Zašto mi ne radi search?",
        Content = "Kada tražim pjesme ne vraća mi rezultate.",
        Status = "Answered",
        Answer = "Provjeri internet konekciju i pokušaj ponovo.",
        CreatedAt = new DateTime(2026, 3, 21, 14, 30, 0),
        AnsweredAt = new DateTime(2026, 3, 21, 15, 10, 0)
    },

    new Question
    {
        Id = 3,
        UserId = 4,
        Title = "Kako promijeniti email?",
        Content = "Želim promijeniti email na svom profilu.",
        Status = "Pending",
        Answer = null,
        CreatedAt = new DateTime(2026, 3, 22, 9, 45, 0),
        AnsweredAt = null
    },

    new Question
    {
        Id = 4,
        UserId = 5,
        Title = "Premium pretplata ne radi",
        Content = "Kupio sam premium ali nemam pristup premium funkcijama.",
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
        Message = "Klikni na tri tačke pored pjesme i izaberi 'Add to playlist'.",
        CreatedAt = new DateTime(2026, 3, 20, 11, 0, 0)
    },

    new Answer
    {
        Id = 2,
        QuestionId = 2,
        AdminId = 1,
        Message = "Provjeri internet konekciju i pokušaj ponovo.",
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

            var userJoinDates = allUsers.ToDictionary(x => x.Id, x => x.JoinDate);

            modelBuilder.Entity<Subscription>().HasData(
                GenerateSubscriptions(3, 260, userJoinDates)
            );
        }

        

        private static List<User> GenerateAdditionalUsers(string userHash, string userSalt, int additionalCount)
        {
            var random = new Random(20260311);
            var users = new List<User>();

            int startId = 11;

            for (int i = 0; i < additionalCount; i++)
            {
                int id = startId + i;

                int year = random.Next(0, 2) == 0 ? 2025 : 2026;
                int month = random.Next(1, 13);
                int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);

                var joinDate = new DateTime(year, month, day);

                users.Add(new User
                {
                    Id = id,
                    FirstName = $"User{id}",
                    LastName = $"Test{id}",
                    Username = $"user{id}",
                    Password = string.Empty,
                    PasswordHash = userHash,
                    PasswordSalt = userSalt,
                    Email = $"user{id}@grooveon.com",
                    UserImage = null,
                    DateOfBirth = new DateTime(
                        random.Next(1995, 2006),
                        random.Next(1, 13),
                        random.Next(1, 28)
                    ),
                    PhoneNumber = $"061{random.Next(100000, 999999)}",
                    IsActive = true,
                    JoinDate = joinDate,
                    LastLogin = null
                });
            }

            return users;
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

        private static List<PlayHistory> GeneratePlayHistories(int totalUsers, int totalSongs, int totalPlayCount = 2000)
        {
            var random = new Random(20260311);
            var playHistories = new List<PlayHistory>();
            int playHistoryId = 1;

            // Pretpostavka:
            // UserId 1 i 2 su admini
            // obični korisnici kreću od 3
            int firstRegularUserId = 3;

            // Malo "popularnijih" pjesama da statistika izgleda realnije
            // (možeš kasnije dodati/izmijeniti koje god želiš)
            var popularSongIds = new List<int>
    {
        9,   // Blinding Lights
        26,  // One Dance
        34,  // Hotline Bling
        56,  // Stronger
        74,  // Faint
        80,  // Numb
        97,  // All Too Well
        109, // Hotel California
        177, // Rolling in the Deep
        198, // thank u, next
        201, // Diamonds
        229, // Father Stretch My Hands Pt. 1
        267, // Snow (Hey Oh)
        300, // No Surprises
        316, // Watermelon Sugar
        342, // Mockingbird
        365, // Just the Way You Are
        397, // As It Was
        408  // Sign of the Times
    };

            DateTime RandomDateInYear(int year)
            {
                int month = random.Next(1, 13);
                int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);
                int hour = random.Next(0, 24);
                int minute = random.Next(0, 60);
                int second = random.Next(0, 60);

                return new DateTime(year, month, day, hour, minute, second);
            }

            int PickSongId()
            {
                // 60% šanse da uzme neku "popularnu" pjesmu
                if (random.NextDouble() < 0.60)
                {
                    return popularSongIds[random.Next(popularSongIds.Count)];
                }

                // 40% šanse da uzme bilo koju pjesmu
                return random.Next(1, totalSongs + 1);
            }

            // 1) Svakom običnom korisniku daj osnovni broj play-eva
            //    da statistika po korisniku ne bude prazna
            for (int userId = firstRegularUserId; userId <= totalUsers; userId++)
            {
                int playsForUser = random.Next(8, 21); // 8-20 playeva po useru

                for (int i = 0; i < playsForUser; i++)
                {
                    int year = random.NextDouble() < 0.50 ? 2025 : 2026;

                    playHistories.Add(new PlayHistory
                    {
                        Id = playHistoryId++,
                        UserId = userId,
                        SongId = PickSongId(),
                        PlayedAt = RandomDateInYear(year)
                    });
                }
            }

            // 2) Dopuni do traženog broja zapisa
            while (playHistories.Count < totalPlayCount)
            {
                int userId = random.Next(firstRegularUserId, totalUsers + 1);
                int year = random.NextDouble() < 0.45 ? 2025 : 2026;

                playHistories.Add(new PlayHistory
                {
                    Id = playHistoryId++,
                    UserId = userId,
                    SongId = PickSongId(),
                    PlayedAt = RandomDateInYear(year)
                });
            }

            // 3) Sortiraj po datumu da izgleda urednije
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

        private static List<Subscription> GenerateSubscriptions(
            int startUserId,
            int endUserId,
            Dictionary<int, DateTime> userJoinDates)
        {
            var random = new Random(20260312);
            var subscriptions = new List<Subscription>();
            int subscriptionId = 1;

            for (int userId = startUserId; userId <= endUserId; userId++)
            {
                if (!userJoinDates.ContainsKey(userId))
                    continue;

                var joinDate = userJoinDates[userId];

                subscriptions.Add(new Subscription
                {
                    Id = subscriptionId++,
                    UserId = userId,
                    SubscriptionPlanId = 1,
                    StartDate = joinDate,
                    ExpiryDate = joinDate,
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                });

                int premiumCount = random.Next(2, 8);

                for (int i = 0; i < premiumCount; i++)
                {
                    int year = random.Next(0, 2) == 0 ? 2025 : 2026;
                    int maxMonth = GetMaxAllowedMonthForYear(year);

                    if (maxMonth <= 0)
                        continue;

                    int month = random.Next(1, maxMonth + 1);
                    int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);

                    var paymentDate = new DateTime(year, month, day);

                    if (paymentDate < joinDate)
                        paymentDate = joinDate;

                    if (paymentDate.Year != year)
                        continue;

                    if (paymentDate.Year == DateTime.Today.Year &&
                        paymentDate.Month >= DateTime.Today.Month)
                        continue;

                    float amount = random.Next(0, 100) < 75 ? 4.99f : 9.99f;
                    string paymentMethod = random.Next(0, 2) == 0 ? "Card" : "PayPal";

                    subscriptions.Add(new Subscription
                    {
                        Id = subscriptionId++,
                        UserId = userId,
                        SubscriptionPlanId = 2,
                        StartDate = paymentDate,
                        ExpiryDate = paymentDate.AddDays(30),
                        IsActive = paymentDate.AddDays(30) >= DateTime.Today,
                        PaymentMethod = paymentMethod,
                        PaymentAmount = amount,
                        PaymentDate = paymentDate
                    });
                }
            }

            return subscriptions;
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