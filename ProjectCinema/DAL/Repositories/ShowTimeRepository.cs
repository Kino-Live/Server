using Microsoft.EntityFrameworkCore;
using ProjectCinema.Data;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Interfaces;

namespace ProjectCinema.Repositories.Classes
{
    public class ShowTimeRepository : GenericRepository<ShowTime>, IShowTimeRepository
    {
        public ShowTimeRepository(AplicationDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ShowTime>> GetShowTimesAsync(ShowTimeStatus? showTimeStatus = null)
        {

            var query = _dbSet.AsQueryable();

            if(showTimeStatus.HasValue)
            {

                query = query.AsNoTracking().Where(s => s.ShowTimeStatus == showTimeStatus.Value);

            }

            return await query.Include(t => t.Tickets).ToListAsync();

        }

        public async Task<IEnumerable<ShowTime>> GetShowTimesByMovieScreeningIdAsync(int id)
        {

            return await _dbContext.ShowTimes
                                    .AsNoTracking()
                                    .Where(s => s.MovieScreeningId == id)
                                    .Include(t => t.Tickets)
                                    .ToListAsync();

        }

        public async Task<IEnumerable<ShowTime>> GetShowTimesByHallIdAsync(int id)
        {

            return await _dbContext.ShowTimes
                                    .AsNoTracking()
                                    .Where(s => s.HallId == id)
                                    .Include(t => t.Tickets)
                                    .ToListAsync();

        }

        public async Task<List<DateOnly>> GetAvailableDatesByMovieIdAsync(int movieId)
        {
            var today = DateTime.Now.Date;

            // Получаем ID актуальных прокатов для фильма
            var relevantScreeningIds = await GetRelevantScreeningIdsByMovieIdAsync(movieId);

            if (!relevantScreeningIds.Any())
            {
                return new List<DateOnly>();
            }

            // Получаем уникальные даты из активных сеансов
            var availableDates = await _dbContext.ShowTimes
                .AsNoTracking()
                .Where(st => relevantScreeningIds.Contains(st.MovieScreeningId) &&
                            st.ShowTimeStatus == ShowTimeStatus.Active &&
                            st.StartTime.Date >= today)
                .Select(st => st.StartTime.Date)
                .Distinct()
                .OrderBy(date => date)
                .ToListAsync();

            // Преобразуем DateTime в DateOnly
            return availableDates.Select(dt => DateOnly.FromDateTime(dt)).ToList();
        }

        public async Task<IEnumerable<ShowTime>> GetShowTimesByMovieIdAndDateAsync(int movieId, DateOnly date)
        {
            var now = DateTime.Now;
            var dateStart = date.ToDateTime(TimeOnly.MinValue);
            var dateEnd = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

            // Получаем ID актуальных прокатов для фильма
            var relevantScreeningIds = await GetRelevantScreeningIdsByMovieIdAsync(movieId);

            if (!relevantScreeningIds.Any())
            {
                return Enumerable.Empty<ShowTime>();
            }

            // Получаем сеансы на указанную дату
            return await _dbContext.ShowTimes
                .AsNoTracking()
                .Where(st => relevantScreeningIds.Contains(st.MovieScreeningId) &&
                            st.ShowTimeStatus == ShowTimeStatus.Active &&
                            st.StartTime >= dateStart &&
                            st.StartTime < dateEnd &&
                            st.StartTime >= now)
                .Include(st => st.Hall)
                    .ThenInclude(h => h!.Cinema)
                .Include(st => st.MovieScreening)
                .OrderBy(st => st.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Получает список ID актуальных прокатов для указанного фильма
        /// </summary>
        private async Task<List<int>> GetRelevantScreeningIdsByMovieIdAsync(int movieId)
        {
            return await _dbContext.MovieScreenings
                .AsNoTracking()
                .Where(ms => ms.MovieId == movieId && 
                            ms.MovieScreeningRelevance == MovieScreeningRelevance.Relevant)
                .Select(ms => ms.MovieScreeningId)
                .ToListAsync();
        }
    }
}
