using Identity.Domain.Entities;

namespace TestWeb.API.Repositories
{
    public interface IAddressRepository
    {
        AddressEntity Save(AddressEntity address);
    }
}