using ProjectCinema.Entities;

namespace ProjectCinema.Repositories.Interfaces
{
    public interface IPasswordResetTokenRepository : IGenericRepository<PasswordResetToken>
    {
        Task<PasswordResetToken> CreateAsync(PasswordResetToken token);
        Task<PasswordResetToken?> FindByTokenHashAsync(string tokenHash);
        Task<bool> MarkAsUsedAsync(int tokenId);
        Task<int> DeleteExpiredAsync();
    }
}

