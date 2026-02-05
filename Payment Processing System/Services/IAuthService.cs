using Microsoft.EntityFrameworkCore;
using Payment_Processing_System.Data;
using PPS.Common;

namespace Payment_Processing_System.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AuthService(ApplicationDbContext context, IJwtTokenGenerator tokenGenerator)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return null;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "User",
                CreatedAt = DateTime.UtcNow
                ,Id = Guid.NewGuid()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Создаём счёт для нового пользователя
            var account = new Account
            {
                AccountId = $"acc_{Guid.NewGuid().ToString("N")[..8]}",
                UserId = user.Id,
                Balance = 5000,
                Currency = "USD",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var token = _tokenGenerator.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
                , AccountId = account.AccountId
            };
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            var token = _tokenGenerator.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };
        }
    }
}
