using webapi.Entities;

namespace webapi.Services
{
    public interface IUserRepository
    {

        Task<bool> UserExistsAsync(string username);

        Task AddNewUser(User newUser);

        Task<bool> SaveChangesAsync();

     

    }
}
