using Identity.Domain.Entities;

namespace TestWeb.API.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public AddressEntity Save(AddressEntity address)
        {
            _context.Addresses.Add(address);
            _context.SaveChanges();
            return address;
        }
    }
}