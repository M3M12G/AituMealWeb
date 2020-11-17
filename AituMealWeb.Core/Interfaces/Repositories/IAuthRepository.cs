using AituMealWeb.Core.Entities;
using System;
using System.Threading.Tasks;

namespace AituMealWeb.Core.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task<Guid> Register(User userDetails, string password);
        Task<User> Login(string email, string password);
        bool UserExists(string email);
    }
}
