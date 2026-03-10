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
        public DbSet<ListeningHistory> ListeningHistories { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin123",
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
                    Username = "admin456",
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
                    Username = "user1234",
                    Password = string.Empty,
                    PasswordHash = userHash,
                    PasswordSalt = userSalt,
                    Email = "user1@grooveon.com",
                    UserImage = null,
                    DateOfBirth = new DateTime(2002, 5, 21),
                    PhoneNumber = "061111113",
                    IsActive = true,
                    JoinDate = new DateTime(2026, 3, 10),
                    LastLogin = null
                },
                new User
                {
                    Id = 4,
                    Username = "music2026",
                    Password = string.Empty,
                    PasswordHash = userHash,
                    PasswordSalt = userSalt,
                    Email = "user2@grooveon.com",
                    UserImage = null,
                    DateOfBirth = new DateTime(1999, 9, 10),
                    PhoneNumber = "061111114",
                    IsActive = true,
                    JoinDate = new DateTime(2026, 3, 10),
                    LastLogin = null
                },
                new User
                {
                    Id = 5,
                    Username = "groove789",
                    Password = string.Empty,
                    PasswordHash = userHash,
                    PasswordSalt = userSalt,
                    Email = "user3@grooveon.com",
                    UserImage = null,
                    DateOfBirth = new DateTime(2001, 3, 14),
                    PhoneNumber = "061111115",
                    IsActive = true,
                    JoinDate = new DateTime(2026, 3, 10),
                    LastLogin = null
                },
                new User
                {
                    Id = 6,
                    Username = "playlist1",
                    Password = string.Empty,
                    PasswordHash = userHash,
                    PasswordSalt = userSalt,
                    Email = "user4@grooveon.com",
                    UserImage = null,
                    DateOfBirth = new DateTime(2003, 7, 8),
                    PhoneNumber = "061111116",
                    IsActive = true,
                    JoinDate = new DateTime(2026, 3, 10),
                    LastLogin = null
                },
                new User
                {
                    Id = 7,
                    Username = "album2025",
                    Password = string.Empty,
                    PasswordHash = userHash,
                    PasswordSalt = userSalt,
                    Email = "user5@grooveon.com",
                    UserImage = null,
                    DateOfBirth = new DateTime(1997, 11, 2),
                    PhoneNumber = "061111117",
                    IsActive = true,
                    JoinDate = new DateTime(2026, 3, 10),
                    LastLogin = null
                },
                new User
                {
                    Id = 8,
                    Username = "rockstar9",
                    Password = string.Empty,
                    PasswordHash = userHash,
                    PasswordSalt = userSalt,
                    Email = "user6@grooveon.com",
                    UserImage = null,
                    DateOfBirth = new DateTime(1996, 12, 25),
                    PhoneNumber = "061111118",
                    IsActive = true,
                    JoinDate = new DateTime(2026, 3, 10),
                    LastLogin = null
                },
                new User
                {
                    Id = 9,
                    Username = "indie2024",
                    Password = string.Empty,
                    PasswordHash = userHash,
                    PasswordSalt = userSalt,
                    Email = "user7@grooveon.com",
                    UserImage = null,
                    DateOfBirth = new DateTime(2004, 1, 9),
                    PhoneNumber = "061111119",
                    IsActive = true,
                    JoinDate = new DateTime(2026, 3, 10),
                    LastLogin = null
                },
                new User
                {
                    Id = 10,
                    Username = "listener8",
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
            );

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = 1, UserId = 1, RoleId = 1, DateAssigned = new DateTime(2026, 3, 10) },
                new UserRole { Id = 2, UserId = 2, RoleId = 1, DateAssigned = new DateTime(2026, 3, 10) },

                new UserRole { Id = 3, UserId = 3, RoleId = 2, DateAssigned = new DateTime(2026, 3, 10) },
                new UserRole { Id = 4, UserId = 4, RoleId = 2, DateAssigned = new DateTime(2026, 3, 10) },
                new UserRole { Id = 5, UserId = 5, RoleId = 2, DateAssigned = new DateTime(2026, 3, 10) },
                new UserRole { Id = 6, UserId = 6, RoleId = 2, DateAssigned = new DateTime(2026, 3, 10) },
                new UserRole { Id = 7, UserId = 7, RoleId = 2, DateAssigned = new DateTime(2026, 3, 10) },
                new UserRole { Id = 8, UserId = 8, RoleId = 2, DateAssigned = new DateTime(2026, 3, 10) },
                new UserRole { Id = 9, UserId = 9, RoleId = 2, DateAssigned = new DateTime(2026, 3, 10) },
                new UserRole { Id = 10, UserId = 10, RoleId = 2, DateAssigned = new DateTime(2026, 3, 10) }
            );

            modelBuilder.Entity<SubscriptionPlan>().HasData(
                new SubscriptionPlan
                {
                    Id = 1,
                    Name = "Free",
                    Price = 0,
                    DurationDays = 0,
                    Description = "Osnovni plan sa preview pristupom",
                    IsActive = true
                },
                new SubscriptionPlan
                {
                    Id = 2,
                    Name = "Premium",
                    Price = 4.99f,
                    DurationDays = 30,
                    Description = "Premium plan za 30 dana",
                    IsActive = true
                },
                new SubscriptionPlan
                {
                    Id = 3,
                    Name = "Premium Plus",
                    Price = 9.99f,
                    DurationDays = 30,
                    Description = "Napredni plan za 30 dana",
                    IsActive = true
                }
            );
        }
    }
}