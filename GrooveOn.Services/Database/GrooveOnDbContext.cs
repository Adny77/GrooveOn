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
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Korisnik" }
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