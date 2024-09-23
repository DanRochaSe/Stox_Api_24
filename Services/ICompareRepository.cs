using webapi.Entities;

namespace webapi.Services
{
    public interface ICompareRepository
    {

        Task<IEnumerable<StoxComparison>> GetStoxComparisons();

        Task<StoxComparison> GetStoxComparisonByID(int comparisonId);

        Task<IEnumerable<StoxComparison>> GetStoxComparisonsByUserId(int userId);

        Task<IEnumerable<StoxComparison>> GetStoxComparisonsByUsername(string? username);

        Task<IEnumerable<Stox>> GetStoxInComparison(int comparisonId);

        Task AddComparison(StoxComparison newComparison);

        Task<bool> SaveChangesAsync();

        Task<bool> ComparisonExistsAsync(int comparisonId);

        Task DeleteComparison(StoxComparison comparison);
    }
}
