using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using Identity.Domain.Managers;
using Identity.Domain.Models;
using TestWeb.API.Repositories;

namespace TestWeb.API.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;

        public UserManager(IUserRepository userRepository, IAddressRepository addressRepository)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
        }

        public Task<UserEntity> GetByLogin(string login)
        {
            return Task.FromResult(_userRepository.Get().Result.First(u => u.Login == login));
        }

        public Task<IEnumerable<UserEntity>> GetByCountry(string country)
        {
            return Task.FromResult(_userRepository.Get().Result.Where(u => u.AddressEntity.Country == country));
        }

        public Task Create(UserRequest request)
        {
            AddressEntity addressEntity = _addressRepository.Save(new AddressEntity
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
            });

            _userRepository.Save(new UserEntity
            {
                AddressEntity = addressEntity,
                Login = request.Login,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName
            });

            return Task.CompletedTask;
        }
    }
}