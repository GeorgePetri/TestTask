using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using Identity.Domain.Managers;
using Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Persistence.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationDbContext> _logger;

        public UserManager(ApplicationDbContext context, ILogger<ApplicationDbContext> logger)
        {
            _context = context;
            _logger = logger;
        }

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

        public async Task<UserEntity> Create(UserRequest request)
        {
            var entity = new UserEntity
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
            };

            _context.Users.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Create failed for: {entity}", entity);
                return null;
            }

            return entity;
        }

        private IQueryable<UserEntity> WithIncludeAddress() =>
            _context.Users.Include(u => u.AddressEntity);
    }
}