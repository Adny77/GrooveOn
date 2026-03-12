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

            var additionalUsers = GenerateAdditionalUsers(userHash, userSalt, 250);
            var allUsers = baseUsers.Concat(additionalUsers).ToList();

            modelBuilder.Entity<User>().HasData(allUsers);

            modelBuilder.Entity<UserRole>().HasData(GenerateUserRoles(260));

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