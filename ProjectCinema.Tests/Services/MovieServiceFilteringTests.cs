using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectCinema.BLL.DTO.Movie;
using ProjectCinema.BLL.Services;
using ProjectCinema.Data;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Classes;
using ProjectCinema.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCinema.Tests.Services
{
    public class MovieServiceFilteringTests : IDisposable
    {
        private readonly AplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IMovieRepository _movieRepository;
        private readonly MovieService _movieService;

        public MovieServiceFilteringTests()
        {
            var options = new DbContextOptionsBuilder<AplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AplicationDBContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProjectCinema.MappingProfiles.MovieMappingProfile>();
            });
            _mapper = config.CreateMapper();

            _movieRepository = new MovieRepository(_context);
            _movieService = new MovieService(_movieRepository, _mapper, null!);

            SeedMoviesAsync().Wait();
        }

        private async Task SeedMoviesAsync()
        {
            var movies = new List<Movie>
            {
                new Movie
                {
                    MovieName = "Dune",
                    Description = "A sci-fi epic about Arrakis.",
                    ReleaseYear = new DateOnly(2021, 1, 1),
                    Genre = "Sci-Fi",
                    Language = "English",
                    AgeRestriction = 13,
                    Url = "https://example.com/dune",
                    Status = StatusOfMovie.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Movie
                {
                    MovieName = "Inception",
                    Description = "A dream within a dream.",
                    ReleaseYear = new DateOnly(2010, 1, 1),
                    Genre = "Action",
                    Language = "English",
                    AgeRestriction = 13,
                    Url = "https://example.com/inception",
                    Status = StatusOfMovie.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Movie
                {
                    MovieName = "Matrix",
                    Description = "yyyyyyy",
                    ReleaseYear = new DateOnly(1999, 1, 1),
                    Genre = "Sci-Fi",
                    Language = "English",
                    AgeRestriction = 18,
                    Url = "https://example.com/matrix",
                    Status = StatusOfMovie.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await _context.Movies.AddRangeAsync(movies);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetFilteredMoviesAsync_ShouldReturnPagedResult()
        {
            var filter = new MovieFilterRequestDTO
            {
                Page = 1,
                PageSize = 2
            };

            var result = await _movieService.GetFilteredMovieAsync(filter, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(3, result.Total);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(1, result.Page);
            Assert.Equal(2, result.PageSize);
        }

        [Fact]
        public async Task GetFilteredMoviesAsync_ShouldBeSortedByReleaseYearDesc()
        {
            var filter = new MovieFilterRequestDTO
            {
                Page = 1,
                PageSize = 3
            };

            var result = await _movieService.GetFilteredMovieAsync(filter, CancellationToken.None);
            var years = result.Items.Select(m => m.ReleaseYear).ToList();

            Assert.True(years.SequenceEqual(years.OrderByDescending(y => y)),
                "Movies should be sorted by ReleaseYear descending");
        }

        [Fact]
        public async Task GetFilteredMoviesAsync_ShouldMapFieldsCorrectly()
        {
            var filter = new MovieFilterRequestDTO
            {
                Page = 1,
                PageSize = 3
            };

            var result = await _movieService.GetFilteredMovieAsync(filter, CancellationToken.None);
            var movie = result.Items.First();

            Assert.NotNull(movie.MovieName);
            Assert.IsType<int>(movie.ReleaseYear);
            Assert.NotNull(movie.Genre);
            Assert.NotNull(movie.Language);
        }

        public void Dispose() => _context.Dispose();
    }
}
