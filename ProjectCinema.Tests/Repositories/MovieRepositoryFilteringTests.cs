using Microsoft.EntityFrameworkCore;
using ProjectCinema.DAL.Models;
using ProjectCinema.Data;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCinema.Tests.Repositories
{
    public class MovieRepositoryFilteringTests
    {
        private AplicationDBContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AplicationDBContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            var context = new AplicationDBContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private async Task SeedMoviesAsync(AplicationDBContext context)
        {
            var m1 = new Movie
            {
                MovieName = "The Matrix",
                Description = "A computer hacker learns about the true nature of reality",
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
                MainCast = "Keanu Reeves, Laurence Fishburne",
                Status = StatusOfMovie.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var m2 = new Movie
            {
                MovieName = "Inception",
                Description = "A thief who steals corporate secrets",
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
                MainCast = "Leonardo DiCaprio, Marion Cotillard",
                Status = StatusOfMovie.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var m3 = new Movie
            {
                MovieName = "Interstellar",
                Description = "A team of explorers travel through a wormhole",
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
                MainCast = "Matthew McConaughey, Anne Hathaway",
                Status = StatusOfMovie.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var m4 = new Movie
            {
                MovieName = "Avatar",
                Description = "A paraplegic marine dispatched to the moon Pandora",
                DurationInMinutes = 162,
                AgeRestriction = 13,
                GlobalStartDate = DateTime.UtcNow.AddDays(-5),
                GlobalEndDate = DateTime.UtcNow.AddDays(25),
                Url = "https://example.com/avatar",
                ReleaseYear = new DateOnly(2009, 1, 1),
                Genre = "Comedy",
                Language = "English",
                ProductionStudio = "20th Century Fox",
                Director = "James Cameron",
                MainCast = "Sam Worthington, Zoe Saldana",
                Status = StatusOfMovie.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Movies.AddRangeAsync(m1, m2, m3, m4);
            await context.SaveChangesAsync();

            context.Reviews.AddRange(
                new Review { MovieId = m1.MovieId, Rating = 9, CreatedAt = DateTime.UtcNow },
                new Review { MovieId = m2.MovieId, Rating = 8, CreatedAt = DateTime.UtcNow },
                new Review { MovieId = m4.MovieId, Rating = 5, CreatedAt = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
        }

        [Fact]
        public async Task Filter_By_Genres_Returns_Correct_Movies()
        {
            using var context = CreateInMemoryContext();
            await SeedMoviesAsync(context);
            var repository = new MovieRepository(context);

            var result = repository.BuildFilteredQuery(new MovieFilterParams
            {
                Genres = new[] { "Drama", "Comedy"}
            }).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, m => m.MovieName == "Interstellar");
            Assert.Contains(result, m => m.MovieName == "Avatar");
        }

        [Fact]
        public async Task Filter_By_Year_Range_Works_Inclusively()
        {
            using var context = CreateInMemoryContext();
            await SeedMoviesAsync(context);
            var repository = new MovieRepository(context);

            var result = repository.BuildFilteredQuery(new MovieFilterParams
            {
                YearFrom = 1999,
                YearTo = 2010
            }).ToList();

            Assert.Equal(3, result.Count);
            Assert.All(result, m =>
            Assert.True(m.ReleaseYear.Year >= 1999 && m.ReleaseYear.Year <= 2010));
        }

        [Fact]
        public async Task Filter_By_RatingMin_Returns_Only_HighRated()
        {
            using var context = CreateInMemoryContext();
            await SeedMoviesAsync(context);
            var repository = new MovieRepository(context);

            var result = repository.BuildFilteredQuery(new MovieFilterParams
            {
                RatingMin = 8
            }).ToList();

            Assert.Equal(2, result.Count);
            Assert.DoesNotContain(result, m => m.MovieName == "Avatar");
        }

        [Fact]
        public async Task Combined_Filters_Returns_One_Movie()
        {
            using var context = CreateInMemoryContext();
            await SeedMoviesAsync(context);
            var repo = new MovieRepository(context);

            var result = repo.BuildFilteredQuery(new MovieFilterParams
            {
                Genres = new[] { "Sci-Fi" },
                YearFrom = 2000,
                RatingMin = 8
            }).ToList();

            Assert.Single(result);
            Assert.Equal("Inception", result.First().MovieName);
        }
    }
}
