using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using webapi.Data;
using webapi.Entities;
using webapi.Models;

namespace webapi.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IConfiguration _config;
        private readonly StoxContext _context;

        public AuthenticationRepository(IConfiguration config, StoxContext context)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<User> GetUserByID(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByUsername(string? username)
        {
            return await _context.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUsernameAndPassword(string? username, string? password)
        {
            return await _context.Users.Where(u => u.UserName == username && u.PasswordHash == password).FirstOrDefaultAsync();
        }

        public async Task<string> GetUserSalt(string username)
        {
            var userSalt = await _context.UserSalts.Where(u => u.UserName == username).FirstOrDefaultAsync();
            if (userSalt == null)
            {
                return "";
            }

            return userSalt?.Salt;
        }


        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0); 
        }
    }
}
