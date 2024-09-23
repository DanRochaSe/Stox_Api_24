using System.Collections.Generic;
using webapi.Entities;
using webapi.Models;

namespace webapi.Services
{
    public interface IStoxRepository
    {

        Task<IEnumerable<Stox>> GetAllStoxAsync();

        Task AddStox(Stox stox);

        Task<bool> SaveChangesAsync();

        Task<Stox> GetStoxById(int stoxId);

        Task<IEnumerable<Stox>> GetStoxByUser(int userId);

        Task<bool> StoxExistsAsync(int stoxId);
    }
}
