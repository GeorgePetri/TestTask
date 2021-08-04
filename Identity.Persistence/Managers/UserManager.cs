using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using Identity.Domain.Managers;
using Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Persistence.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ApplicationDbContext _context;

        public UserManager(ApplicationDbContext context) => _context = context;

        public Task<UserEntity> GetById(long id) =>
            WithIncludeAddress()
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<UserEntity> GetByLogin(string login) =>
            await WithIncludeAddress()
                .FirstOrDefaultAsync(u => u.Login == login);

        public async Task<IEnumerable<UserEntity>> GetByCountry(string country) =>
            await WithIncludeAddress()
                .Where(u => u.AddressEntity.Country == country)
                .ToListAsync();

        public async Task Create(UserRequest request)
        {
            _context.Users.Add(new UserEntity
            {
                Login = request.Login,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                AddressEntity = new AddressEntity
                {
                    Country = request.Country,
                    City = request.City,
                    Street = request.Street,
                    StreetNumber = request.StreetNumber,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    PostalCode = request.PostalCode,
                    State = request.State,
                    FlatNumber = request.FlatNumber
                }
            });

            await _context.SaveChangesAsync();
        }

        private IQueryable<UserEntity> WithIncludeAddress() =>
            _context.Users.Include(u => u.AddressEntity);
    }
}