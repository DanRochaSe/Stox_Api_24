using System.Collections;
using webapi.Entities;
using webapi.Models;

namespace webapi.Services
{
    public interface IGoalRepository
    {
        Task<IEnumerable<Goals>> GetGoalsByUserID(int userID);

        Task<IEnumerable<Goals>> GetGoalsByUserName(string username);

        Task<Goals> GetGoalByGoalID(int goalID);
        Task<IEnumerable<Goals>> GetAllGoals();

        Task AddGoal(Goals newGoal);

        Task<bool> SaveChangesAsync();

        Task<bool> GoalExistAsync(int goalId);

        Task DeleteGoalAsync(Goals goal);



    }
}
