using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Entities;
using webapi.Models;

namespace webapi.Services
{
    public class GoalRepository : IGoalRepository
    {

        private readonly StoxContext _context;

        public GoalRepository(StoxContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddGoal(Goals newGoal)
        {
            await _context.Goals.AddAsync(newGoal); 
        }

        public async Task DeleteGoalAsync(Goals goal)
        {
            _context.Goals.Remove(goal);
        }

        public async Task<IEnumerable<Goals>> GetAllGoals()
        {
            return await _context.Goals.OrderBy(g => g.DateAdded).ToListAsync();
        }

        public async Task<Goals> GetGoalByGoalID(int goalID)
        {
            return await _context.Goals.Where(g => g.GoalID == goalID).FirstOrDefaultAsync(); 
        }

        public Task<IEnumerable<Goals>> GetGoalsByUserID(int userID)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Goals>> GetGoalsByUserName(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> GoalExistAsync(int goalId)
        {
            return await _context.Goals.AnyAsync(g => g.GoalID == goalId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
