using Microsoft.EntityFrameworkCore;
using Payment_Processing_System.Data;
using PPS.Common;

namespace Payment_Processing_System.Services
{
    public interface IAccountService
    {
        Task<Account?> GetAccountAsync(string accountId);
        Task<Account> CreateAccountAsync(Guid userId);
    }

    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetAccountAsync(string accountId)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
           return null;
        }

        public async Task<Account> CreateAccountAsync(Guid userId)
        {
            var account = new Account
            {
                AccountId = $"acc_{Guid.NewGuid().ToString("N")[..8]}",
                UserId = userId,
                Balance = 5000,
                Currency = "USD",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return account;
            return null;
        }

        public async Task<bool> UpdateBalanceAsync(string accountId, decimal amount)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
                return false;

            account.Balance += amount;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
