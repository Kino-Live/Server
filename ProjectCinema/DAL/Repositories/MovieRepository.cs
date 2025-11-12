using Microsoft.EntityFrameworkCore;
using ProjectCinema.DAL.Models;
using ProjectCinema.Data;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Interfaces;

namespace ProjectCinema.Repositories.Classes
{
    public class MovieRepository : GenericRepository<Movie>, IMovieRepository
    {
        public MovieRepository(AplicationDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Movie>> GetMoviesByStatusAsync(StatusOfMovie? movieStatus = null)
        {
            var query = _dbSet.AsQueryable();

            if (movieStatus.HasValue)
            {
                query = query.AsNoTracking().Where(m => m.Status == movieStatus.Value);
            }
            return await query.ToListAsync();
        }

        public IQueryable<Movie> BuildFilteredQuery(MovieFilterParams filter)
        {
            var query = _dbSet
                .Include(m => m.Reviews)
                .AsNoTracking()
                .Where(m => m.Status == StatusOfMovie.Active)
                .AsQueryable();

            if (filter.Genres != null && filter.Genres.Any())
            {
                var genres = filter.Genres
                    .Where(g => !string.IsNullOrWhiteSpace(g))
                    .Select(g => g.Trim().ToLower())
                    .ToList();

                query = query.AsEnumerable()
                    .Where(m => m.Genre != null && genres.Any(g => m.Genre.Contains(g, StringComparison.OrdinalIgnoreCase)))
                    .AsQueryable();
            }

            if (filter.YearFrom.HasValue)
            {
                int yearFrom = filter.YearFrom.Value;
                query = query.Where(m => m.ReleaseYear.Year >= yearFrom);
            }

            if (filter.YearTo.HasValue)
            {
                int yearTo = filter.YearTo.Value;
                query = query.Where(m => m.ReleaseYear.Year <= yearTo);
            }

            if (filter.RatingMin.HasValue)
            {
                double min = filter.RatingMin.Value;
                query = query
                    .AsEnumerable()
                    .Where(m => m.Reviews != null &&
                                m.Reviews.Any() &&
                                m.Reviews.Average(r => r.Rating) >= min)
                    .AsQueryable();
            }

            if (filter.RatingMax.HasValue)
            {
                double max = filter.RatingMax.Value;
                query = query
                    .AsEnumerable()
                    .Where(m => m.Reviews != null && m.Reviews.Any() && m.Reviews.Average(r => r.Rating) <= max)
                    .AsQueryable();
            }
            return query;
        }
    }
}
