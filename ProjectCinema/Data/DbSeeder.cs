using Microsoft.EntityFrameworkCore;
using ProjectCinema.Entities;
using ProjectCinema.Enums;

namespace ProjectCinema.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AplicationDBContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed only Movies
            await SeedMoviesAsync(context);
        }

        private static async Task SeedMoviesAsync(AplicationDBContext context)
        {
            if (await context.Movies.AnyAsync())
                return;

            var movies = new List<Movie>
            {
                new Movie
                {
                    MovieName = "The Matrix",
                    Description = "A computer hacker learns about the true nature of reality and his role in the war against its controllers.",
                    DurationInMinutes = 136,
                    AgeRestriction = 18,
                    GlobalStartDate = DateTime.UtcNow.AddDays(-30),
                    GlobalEndDate = DateTime.UtcNow.AddDays(30),
                    Url = "https://example.com/matrix",
                    ReleaseYear = new DateOnly(1999, 1, 1),
                    Genre = "Sci-Fi, Action",
                    Language = "English",
                    ProductionStudio = "Warner Bros.",
                    Director = "The Wachowskis",
                    MainCast = "Keanu Reeves, Laurence Fishburne, Carrie-Anne Moss",
                    Status = StatusOfMovie.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Movie
                {
                    MovieName = "Inception",
                    Description = "A thief who steals corporate secrets through dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                    DurationInMinutes = 148,
                    AgeRestriction = 13,
                    GlobalStartDate = DateTime.UtcNow.AddDays(-20),
                    GlobalEndDate = DateTime.UtcNow.AddDays(40),
                    Url = "https://example.com/inception",
                    ReleaseYear = new DateOnly(2010, 1, 1),
                    Genre = "Sci-Fi, Action",
                    Language = "English",
                    ProductionStudio = "Warner Bros.",
                    Director = "Christopher Nolan",
                    MainCast = "Leonardo DiCaprio, Marion Cotillard, Tom Hardy",
                    Status = StatusOfMovie.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Movie
                {
                    MovieName = "Interstellar",
                    Description = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
                    DurationInMinutes = 169,
                    AgeRestriction = 13,
                    GlobalStartDate = DateTime.UtcNow.AddDays(-10),
                    GlobalEndDate = DateTime.UtcNow.AddDays(50),
                    Url = "https://example.com/interstellar",
                    ReleaseYear = new DateOnly(2014, 1, 1),
                    Genre = "Sci-Fi, Drama",
                    Language = "English",
                    ProductionStudio = "Paramount Pictures",
                    Director = "Christopher Nolan",
                    MainCast = "Matthew McConaughey, Anne Hathaway, Jessica Chastain",
                    Status = StatusOfMovie.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Movie
                {
                    MovieName = "Avatar",
                    Description = "A paraplegic marine dispatched to the moon Pandora on a unique mission becomes torn between following his orders and protecting the world he feels is his home.",
                    DurationInMinutes = 162,
                    AgeRestriction = 13,
                    GlobalStartDate = DateTime.UtcNow.AddDays(-5),
                    GlobalEndDate = DateTime.UtcNow.AddDays(25),
                    Url = "https://example.com/avatar",
                    ReleaseYear = new DateOnly(2009, 1, 1),
                    Genre = "Sci-Fi, Action",
                    Language = "English",
                    ProductionStudio = "20th Century Fox",
                    Director = "James Cameron",
                    MainCast = "Sam Worthington, Zoe Saldana, Sigourney Weaver",
                    Status = StatusOfMovie.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Movie
                {
                    MovieName = "Dune",
                    Description = "Feature adaptation of Frank Herbert's science fiction novel about the son of a noble family entrusted with the protection of the most valuable asset in the galaxy.",
                    DurationInMinutes = 155,
                    AgeRestriction = 13,
                    GlobalStartDate = DateTime.UtcNow.AddDays(5),
                    GlobalEndDate = DateTime.UtcNow.AddDays(35),
                    Url = "https://example.com/dune",
                    ReleaseYear = new DateOnly(2021, 1, 1),
                    Genre = "Sci-Fi, Adventure",
                    Language = "English",
                    ProductionStudio = "Warner Bros.",
                    Director = "Denis Villeneuve",
                    MainCast = "Timothée Chalamet, Rebecca Ferguson, Oscar Isaac",
                    Status = StatusOfMovie.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Movies.AddRange(movies);
            await context.SaveChangesAsync();
        }
    }
}