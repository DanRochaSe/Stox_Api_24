using Azure.Core;
using webapi.Entities;
using webapi.Models;

namespace webapi.Services
{
    public interface IAuthenticationRepository
    {

        Task<User> GetUserByUsername(string? username);


        Task<User> GetUserByID(int userId);

        Task<User> GetUserByUsernameAndPassword(string? username, string? password);

        Task<string> GetUserSalt(string username);


        Task<bool> SaveChangesAsync();

    }
}
