using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using Identity.Domain.Managers;
using Identity.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace TestWeb.API.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public UsersController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("GetUserById")]
        public async Task<UserEntity> GetById(long id)
        {
            return await _userManager.GetById(id);
        }

        [HttpGet("GetUserByLogin")]
        public UserEntity GetByLogin(string login)
        {
            return _userManager.GetByLogin(login).Result;
        }

        [HttpGet("GetUsersByCountry")]
        public IEnumerable<UserEntity> GetByCountry(string country)
        {
            return _userManager.GetByCountry(country).Result;
        }

        [HttpPost("CreateUser")]
        public void Create([FromQuery] UserRequest request)
        {
            _userManager.Create(request);
        }
    }
}