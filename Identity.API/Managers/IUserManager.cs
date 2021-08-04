using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using TestWeb.API.Models;

namespace TestWeb.API.Managers
{
    public interface IUserManager
    {
        Task<UserEntity> GetByLogin(string login);
        
        Task<IEnumerable<UserEntity>> GetByCountry(string country);
        
        Task Create(UserRequest request);
    }
}