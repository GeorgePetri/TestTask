using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Domain.Entities;

namespace TestWeb.API.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> Get();

        UserEntity Save(UserEntity user);
    }
}