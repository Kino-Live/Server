using Microsoft.EntityFrameworkCore;
using ProjectCinema.Data;
using ProjectCinema.Entities;
using ProjectCinema.Repositories.Interfaces;

namespace ProjectCinema.Repositories.Classes
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {

        public UserRepository(AplicationDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int id)
        {
            return await _dbContext.Bookings.AsNoTracking().Where(b => b.UserId == id).ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
    }
}
