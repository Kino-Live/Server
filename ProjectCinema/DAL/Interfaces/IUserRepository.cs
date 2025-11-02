using ProjectCinema.Entities;

namespace ProjectCinema.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User> 
    {
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
    }
}
