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
                    JoinDate = new DateTime(2026, 3, 10),
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
                    JoinDate = new DateTime(2026, 3, 10),
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
                    JoinDate = new DateTime(2026, 3, 10),
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
                    JoinDate = new DateTime(2026, 3, 10),
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
                    JoinDate = new DateTime(2026, 3, 10),
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
                    JoinDate = new DateTime(2026, 3, 10),
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
                    JoinDate = new DateTime(2026, 3, 10),
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
            );

            modelBuilder.Entity<User>().HasData(GenerateAdditionalUsers(userHash, userSalt));

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

            modelBuilder.Entity<Subscription>().HasData(
                new Subscription
                {
                    Id = 1,
                    UserId = 1,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 1, 5),
                    ExpiryDate = new DateTime(2026, 2, 5),
                    IsActive = true,
                    PaymentMethod = "Card",
                    PaymentAmount = 4.99f,
                    PaymentDate = new DateTime(2026, 1, 5)
                },
                new Subscription
                {
                    Id = 2,
                    UserId = 2,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 1, 10),
                    ExpiryDate = new DateTime(2026, 2, 10),
                    IsActive = true,
                    PaymentMethod = "PayPal",
                    PaymentAmount = 9.99f,
                    PaymentDate = new DateTime(2026, 1, 10)
                },
                new Subscription
                {
                    Id = 3,
                    UserId = 3,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 1, 12),
                    ExpiryDate = new DateTime(2026, 1, 12),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 4,
                    UserId = 4,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 1, 15),
                    ExpiryDate = new DateTime(2026, 1, 15),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 5,
                    UserId = 5,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 2, 2),
                    ExpiryDate = new DateTime(2026, 3, 2),
                    IsActive = true,
                    PaymentMethod = "Card",
                    PaymentAmount = 4.99f,
                    PaymentDate = new DateTime(2026, 2, 2)
                },
                new Subscription
                {
                    Id = 6,
                    UserId = 6,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 2, 8),
                    ExpiryDate = new DateTime(2026, 2, 8),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 7,
                    UserId = 7,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 2, 18),
                    ExpiryDate = new DateTime(2026, 3, 18),
                    IsActive = true,
                    PaymentMethod = "PayPal",
                    PaymentAmount = 9.99f,
                    PaymentDate = new DateTime(2026, 2, 18)
                },
                new Subscription
                {
                    Id = 8,
                    UserId = 8,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 3, 1),
                    ExpiryDate = new DateTime(2026, 3, 1),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 9,
                    UserId = 9,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 3, 9),
                    ExpiryDate = new DateTime(2026, 4, 9),
                    IsActive = true,
                    PaymentMethod = "Card",
                    PaymentAmount = 4.99f,
                    PaymentDate = new DateTime(2026, 3, 9)
                },
                new Subscription
                {
                    Id = 10,
                    UserId = 10,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 3, 20),
                    ExpiryDate = new DateTime(2026, 3, 20),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 11,
                    UserId = 3,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 4, 4),
                    ExpiryDate = new DateTime(2026, 5, 4),
                    IsActive = true,
                    PaymentMethod = "Card",
                    PaymentAmount = 4.99f,
                    PaymentDate = new DateTime(2026, 4, 4)
                },
                new Subscription
                {
                    Id = 12,
                    UserId = 4,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 4, 11),
                    ExpiryDate = new DateTime(2026, 4, 11),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 13,
                    UserId = 5,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 5, 6),
                    ExpiryDate = new DateTime(2026, 6, 6),
                    IsActive = true,
                    PaymentMethod = "PayPal",
                    PaymentAmount = 9.99f,
                    PaymentDate = new DateTime(2026, 5, 6)
                },
                new Subscription
                {
                    Id = 14,
                    UserId = 6,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 5, 14),
                    ExpiryDate = new DateTime(2026, 5, 14),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 15,
                    UserId = 7,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 6, 3),
                    ExpiryDate = new DateTime(2026, 7, 3),
                    IsActive = true,
                    PaymentMethod = "Card",
                    PaymentAmount = 4.99f,
                    PaymentDate = new DateTime(2026, 6, 3)
                },
                new Subscription
                {
                    Id = 16,
                    UserId = 8,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 6, 12),
                    ExpiryDate = new DateTime(2026, 6, 12),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 17,
                    UserId = 9,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 7, 7),
                    ExpiryDate = new DateTime(2026, 8, 7),
                    IsActive = true,
                    PaymentMethod = "PayPal",
                    PaymentAmount = 9.99f,
                    PaymentDate = new DateTime(2026, 7, 7)
                },
                new Subscription
                {
                    Id = 18,
                    UserId = 10,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 7, 21),
                    ExpiryDate = new DateTime(2026, 7, 21),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 19,
                    UserId = 1,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 8, 2),
                    ExpiryDate = new DateTime(2026, 9, 2),
                    IsActive = true,
                    PaymentMethod = "Card",
                    PaymentAmount = 4.99f,
                    PaymentDate = new DateTime(2026, 8, 2)
                },
                new Subscription
                {
                    Id = 20,
                    UserId = 2,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 8, 10),
                    ExpiryDate = new DateTime(2026, 8, 10),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 21,
                    UserId = 3,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 9, 5),
                    ExpiryDate = new DateTime(2026, 10, 5),
                    IsActive = true,
                    PaymentMethod = "Card",
                    PaymentAmount = 4.99f,
                    PaymentDate = new DateTime(2026, 9, 5)
                },
                new Subscription
                {
                    Id = 22,
                    UserId = 4,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 9, 15),
                    ExpiryDate = new DateTime(2026, 9, 15),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 23,
                    UserId = 5,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 10, 4),
                    ExpiryDate = new DateTime(2026, 11, 4),
                    IsActive = true,
                    PaymentMethod = "PayPal",
                    PaymentAmount = 9.99f,
                    PaymentDate = new DateTime(2026, 10, 4)
                },
                new Subscription
                {
                    Id = 24,
                    UserId = 6,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 10, 18),
                    ExpiryDate = new DateTime(2026, 10, 18),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 25,
                    UserId = 7,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 11, 6),
                    ExpiryDate = new DateTime(2026, 12, 6),
                    IsActive = true,
                    PaymentMethod = "Card",
                    PaymentAmount = 4.99f,
                    PaymentDate = new DateTime(2026, 11, 6)
                },
                new Subscription
                {
                    Id = 26,
                    UserId = 8,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 11, 16),
                    ExpiryDate = new DateTime(2026, 11, 16),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                },
                new Subscription
                {
                    Id = 27,
                    UserId = 9,
                    SubscriptionPlanId = 2,
                    StartDate = new DateTime(2026, 12, 8),
                    ExpiryDate = new DateTime(2027, 1, 8),
                    IsActive = true,
                    PaymentMethod = "PayPal",
                    PaymentAmount = 9.99f,
                    PaymentDate = new DateTime(2026, 12, 8)
                },
                new Subscription
                {
                    Id = 28,
                    UserId = 10,
                    SubscriptionPlanId = 1,
                    StartDate = new DateTime(2026, 12, 20),
                    ExpiryDate = new DateTime(2026, 12, 20),
                    IsActive = true,
                    PaymentMethod = null,
                    PaymentAmount = 0,
                    PaymentDate = null
                }
            );
        }

        private static List<User> GenerateAdditionalUsers(string userHash, string userSalt)
        {
            var random = new Random(20260311);
            var users = new List<User>();

            int startId = 11;
            int count = 60;

            for (int i = 0; i < count; i++)
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
                    Username = $"user{id}test{id % 100}",
                    Password = string.Empty,
                    PasswordHash = userHash,
                    PasswordSalt = userSalt,
                    Email = $"user{id}@grooveon.com",
                    UserImage = null,
                    DateOfBirth = new DateTime(
                        1995 + random.Next(10),
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
    }
}