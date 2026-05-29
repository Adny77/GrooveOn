using DotNetEnv;
using GrooveOn.Services.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GrooveOn.Services.Database
{
    public class GrooveOnDbContextFactory : IDesignTimeDbContextFactory<GrooveOnDbContext>
    {
        public GrooveOnDbContext CreateDbContext(string[] args)
        {
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
            if (File.Exists(envPath))
                Env.Load(envPath);

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                ?? throw new InvalidOperationException("Missing env var: CONNECTION_STRING");

            var optionsBuilder = new DbContextOptionsBuilder<GrooveOnDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new GrooveOnDbContext(optionsBuilder.Options);
        }
    }
}