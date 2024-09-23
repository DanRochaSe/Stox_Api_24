using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Entities;

namespace webapi.Services
{
    public class CompareRepository : ICompareRepository
    {
        private readonly StoxContext _context;

        public CompareRepository(StoxContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task AddComparison(StoxComparison newComparison)
        {
            await _context.StoxComparison.AddAsync(newComparison);
        }

        public async Task<bool> ComparisonExistsAsync(int comparisonId)
        {
            return await _context.StoxComparison.AnyAsync(c => c.ComparisonID == comparisonId);
        }

        public async Task DeleteComparison(StoxComparison comparison)
        {
            _context.StoxComparison.Remove(comparison);
        }

        public async Task<StoxComparison> GetStoxComparisonByID(int comparisonId)
        {
            return await _context.StoxComparison.Where(c => c.ComparisonID == comparisonId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StoxComparison>> GetStoxComparisons()
        {
            return await _context.StoxComparison.OrderBy(c => c.CreatedAt).ToListAsync();
        }

        public Task<IEnumerable<StoxComparison>> GetStoxComparisonsByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StoxComparison>> GetStoxComparisonsByUsername(string? username)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Stox>> GetStoxInComparison(int comparisonId)
        {
            var comparison = await _context.StoxComparison.Where(c => c.ComparisonID == comparisonId).FirstOrDefaultAsync();

            var stox1 = await _context.Stox.Where(s => s.StoxID == comparison.StoxID_1).FirstOrDefaultAsync();
            var stox2 = await _context.Stox.Where(s => s.StoxID == comparison.StoxID_2).FirstOrDefaultAsync();

            List<Stox> stoxInComparison = new List<Stox>
            {
                stox1, stox2
            };

            return stoxInComparison;


        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
