using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using Identity.Domain.Models;

namespace Identity.Domain.Managers
{
    public interface IUserManager
    {
        Task<UserEntity> GetById(long id);

        Task<UserEntity> GetByLogin(string login);

        Task<IEnumerable<UserEntity>> GetByCountry(string country);

        Task<UserEntity> Create(UserRequest request);
    }
}