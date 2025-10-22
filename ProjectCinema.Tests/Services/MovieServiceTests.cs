using AutoMapper;
using Moq;
using ProjectCinema.BLL.DTO.Movie;
using ProjectCinema.BLL.DTO.MovieScreening;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.BLL.Interfaces.IMovieScreeningServices;
using ProjectCinema.BLL.Services;
using ProjectCinema.Data;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Classes;
using ProjectCinema.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ProjectCinema.Tests.Services
{
    public class MovieServiceTests : IDisposable
    {
        private readonly AplicationDBContext _context;
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly Mock<IMovieScreeningQueryService> _mockScreeningQueryService;
        private readonly MovieService _movieService;

        public MovieServiceTests()
        {
            var options = new DbContextOptionsBuilder<AplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AplicationDBContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProjectCinema.MappingProfiles.MovieMappingProfile>();
            });
            _mapper = config.CreateMapper();

            _movieRepository = new MovieRepository(_context);

            // Создание мока для IMovieScreeningQueryService
            _mockScreeningQueryService = new Mock<IMovieScreeningQueryService>();
            _mockScreeningQueryService
                .Setup(x => x.GetMovieSreeningsByMovieIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<MovieScreeningDTO>());

            _movieService = new MovieService(_movieRepository, _mapper, _mockScreeningQueryService.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllMovies()
        {
            await SeedTestMovies();

            var result = await _movieService.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public async Task GetMovieDetailsAsync_WithValidId_ShouldReturnMovieDetails()
        {
            // Arrange
            await SeedTestMovies();
            var movieId = 1;

            // Act
            var result = await _movieService.GetMovieDetailsAsync(movieId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(movieId, result.MovieId);
            Assert.Equal("The Matrix", result.MovieName);
            Assert.Equal("A computer hacker learns about the true nature of reality", result.Description);
            Assert.Equal(136, result.DurationInMinutes);
            Assert.Equal(18, result.AgeRestriction);
            Assert.Equal("Sci-Fi, Action", result.Genre);
            Assert.Equal("English", result.Language);
            Assert.Equal("Warner Bros.", result.ProductionStudio);
            Assert.Equal("The Wachowskis", result.Director);
            Assert.Equal("Keanu Reeves, Laurence Fishburne", result.MainCast);
            Assert.Equal(StatusOfMovie.Active, result.Status);
        }

        [Fact]
        public async Task GetMovieDetailsAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            await SeedTestMovies();
            var invalidId = 999;

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _movieService.GetMovieDetailsAsync(invalidId));
        }


        private async Task SeedTestMovies()
        {
            var movies = new List<Movie>
            {
                new Movie
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
                },
                new Movie
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
                },
                new Movie
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
                },
                new Movie
                {
                    MovieName = "Avatar",
                    Description = "A paraplegic marine dispatched to the moon Pandora",
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
                    MainCast = "Sam Worthington, Zoe Saldana",
                    Status = StatusOfMovie.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Movie
                {
                    MovieName = "Dune",
                    Description = "Feature adaptation of Frank Herbert's science fiction novel",
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
                    MainCast = "Timothée Chalamet, Rebecca Ferguson",
                    Status = StatusOfMovie.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _context.Movies.AddRange(movies);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
