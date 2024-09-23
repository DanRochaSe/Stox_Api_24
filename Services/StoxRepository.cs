using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Entities;
using webapi.Models;

namespace webapi.Services
{
    public class StoxRepository : IStoxRepository
    {
        private readonly StoxContext _context;

        public StoxRepository(StoxContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Stox>> GetAllStoxAsync()
        {
            return await _context.Stox.OrderBy(s => s.DateAdded).ToListAsync();
        }


        public async Task AddStox(Stox stox)
        {
            _context.Stox.Add(stox);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task<Stox> GetStoxById(int stoxId)
        {
            return await _context.Stox.Where(s => s.StoxID == stoxId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Stox>> GetStoxByUser(int userId)
        {
            return await _context.Stox.Where(s => s.UserID == userId).ToListAsync();
        }

        public async Task<bool> StoxExistsAsync(int stoxId)
        {
            return await _context.Stox.AnyAsync(s => s.StoxID == stoxId);
        }
    }
}
