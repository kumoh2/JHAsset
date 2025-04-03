// AuthService.cs
using JHAsset.Shared.Data;
using JHAsset.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace JHAsset.Shared.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDbContextFactory<JHAssetContext> _contextFactory;

        public AuthService(IDbContextFactory<JHAssetContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            if (await context.Users.AnyAsync(u => u.Username == username))
                return false;

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password)
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return false;

            return user.PasswordHash == HashPassword(password);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}