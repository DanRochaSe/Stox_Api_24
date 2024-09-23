using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Entities;

namespace webapi.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly StoxContext _context;

        public UserRepository(StoxContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddNewUser(User newUser)
        {
            _context.Users.Add(newUser);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public Task<bool> UserExistsAsync(string username)
        {
            return _context.Users.AnyAsync(u => u.UserName == username);
        }


    }
}
