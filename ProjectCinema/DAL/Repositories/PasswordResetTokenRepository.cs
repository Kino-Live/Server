using Microsoft.EntityFrameworkCore;
using ProjectCinema.Data;
using ProjectCinema.Entities;
using ProjectCinema.Repositories.Interfaces;

namespace ProjectCinema.Repositories.Classes
{
    public class PasswordResetTokenRepository : GenericRepository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(AplicationDBContext context) : base(context)
        {
        }

        public async Task<PasswordResetToken> CreateAsync(PasswordResetToken token)
        {
            await _dbSet.AddAsync(token);
            await _dbContext.SaveChangesAsync();
            return token;
        }

        public async Task<PasswordResetToken?> FindByTokenHashAsync(string tokenHash)
        {
            return await _dbContext.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }

        public async Task<bool> MarkAsUsedAsync(int tokenId)
        {
            var token = await _dbSet.FindAsync(tokenId);
            if (token == null || token.UsedAt != null)
            {
                return false;
            }

            token.UsedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> DeleteExpiredAsync()
        {
            var expiredTokens = await _dbSet
                .Where(t => t.ExpiresAt < DateTime.UtcNow || t.UsedAt != null)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                _dbSet.RemoveRange(expiredTokens);
                await _dbContext.SaveChangesAsync();
            }

            return expiredTokens.Count;
        }
    }
}

