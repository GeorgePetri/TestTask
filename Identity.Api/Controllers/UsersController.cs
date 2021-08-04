using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using Identity.Domain.Managers;
using Identity.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public UsersController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserEntity>> GetById(long id)
        {
            var result = await _userManager.GetById(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("byLogin/{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserEntity>> GetByLogin(string login)
        {
            var result = await _userManager.GetByLogin(login);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("byCountry/{country}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetByCountry(string country)
        {
            return Ok(await _userManager.GetByCountry(country));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserEntity>> Create([FromBody] UserRequest request)
        {
            var result = await _userManager.Create(request);

            if (result == null)
                return BadRequest();

            return Created($"v1/Users/{result.Id}", result);
        }
    }
}