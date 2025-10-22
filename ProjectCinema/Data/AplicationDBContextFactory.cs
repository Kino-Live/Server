using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProjectCinema.Data
{
    public class AplicationDBContextFactory : IDesignTimeDbContextFactory<AplicationDBContext>
    {
        public AplicationDBContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AplicationDBContext>();
            // Use the same provider as in Program.cs
            optionsBuilder.UseSqlServer(connectionString);
            // If you switch to SQLite, replace with:
            // optionsBuilder.UseSqlite(connectionString);

            return new AplicationDBContext(optionsBuilder.Options);
        }
    }
}


