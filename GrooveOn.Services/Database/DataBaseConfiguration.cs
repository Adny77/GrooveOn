using GrooveOn.Services.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GrooveOn.Services.Database
{
    public class GrooveOnDbContextFactory : IDesignTimeDbContextFactory<GrooveOnDbContext>
    {
        public GrooveOnDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GrooveOnDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=.\\SQLEXPRESS;Database=GrooveOnDb;Trusted_Connection=True;TrustServerCertificate=True;"
            );

            return new GrooveOnDbContext(optionsBuilder.Options);
        }
    }
}