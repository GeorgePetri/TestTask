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
        private readonly IPasswordManager _passwordManager;
        private readonly ILogger<ApplicationDbContext> _logger;

        public UserManager(ApplicationDbContext context, IPasswordManager passwordManager, ILogger<ApplicationDbContext> logger)
        {
            _context = context;
            _passwordManager = passwordManager;
            _logger = logger;
        }

        public async Task<UserEntity> GetById(long id)
        {
            var user = await WithIncludeAddress()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
                HidePassword(user);

            return user;
        }

        public async Task<UserEntity> GetByLogin(string login)
        {
            var user = await WithIncludeAddress()
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user != null)
                HidePassword(user);

            return user;
        }

        public async Task<IEnumerable<UserEntity>> GetByCountry(string country)
        {
            var users = await WithIncludeAddress()
                .Where(u => u.AddressEntity.Country == country)
                .ToListAsync();

            foreach (var user in users)
            {
                HidePassword(user);
            }

            return users;
        }

        public async Task<UserEntity> Create(UserRequest request)
        {
            var hashedPassword = _passwordManager.HashPassword(request.Password);

            var entity = new UserEntity
            {
                Login = request.Login,
                Password = hashedPassword,
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

            HidePassword(entity);

            return entity;
        }

        private IQueryable<UserEntity> WithIncludeAddress() =>
            _context.Users.Include(u => u.AddressEntity);

        private static void HidePassword(UserEntity userEntity)
        {
            userEntity.Password = "Redacted";
        }
    }
}