using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AP.Cryptography;
using AP.Entities.Enums;
using AP.Repositories.User;
using AP.Validators.User;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Eager = AP.Entities.Models.Eager;
using Models = AP.Entities.Models;

namespace AP.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly IMapper _mapper;

        readonly IUserRepository _userRepository;

        public UsersController(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        #region GET

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Eager.User>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Get()
        {
            var users = await _userRepository.GetAllUsers();

            if(users == null || !users.Any())
                return NoContent();

            var usersMapped = _mapper.Map<IEnumerable<Eager.User>>(users);

            return Ok(usersMapped);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(Eager.User))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get(Guid id)
        {
            if(id.Equals(Guid.Empty))
                return BadRequest(CommonResponseMessages.NoId);

            var user = await _userRepository.Retrieve(id);

            if(user == null)
                return NoContent();

            var userMapped = _mapper.Map<Eager.User>(user);

            return Ok(userMapped);
        }

        #endregion GET

        #region POST

        /// <summary>
        /// Create user record
        /// </summary>
        /// <remarks>
        /// In request password field should contains raw user password.
        /// If you get validation error "INVALID_PASSWORD" remember that password need to be strong. What means:
        /// - Two uppercase letters
        /// - One special case letter
        /// - Two digits
        /// - Three lowercase letters
        /// - Length of at least 8
        /// </remarks>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(400, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Post([FromBody] Eager.User user)
        {
            var userMapped = _mapper.Map<Models.User>(user);

            var validationErrors = UserValidator.OnUserCreateValidation(userMapped);
            
            if(validationErrors.Any())
                return BadRequest(validationErrors);

            userMapped.Password = SHA.GenerateSHA256String(userMapped.Password);

            var userCreated = await _userRepository.Create(userMapped);
            return Created($"{this.Request.Scheme}://{this.Request.Host}/api/Users/{userCreated.Id}", null);
        }

        #endregion POST

        #region PUT

        [HttpPut]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400, Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Put([FromBody] Eager.User user)
        {
            if (!user.Id.HasValue)
                return BadRequest();

            if(!_userRepository.Exists(user.Id.Value))
                return NoContent();

            var userMapped = _mapper.Map<Models.User>(user);

            var validationErrors = UserValidator.OnUserCreateValidation(userMapped);

            if(validationErrors.Any())
                return BadRequest(validationErrors);

            await _userRepository.Update(userMapped);
            return Ok();
        }

        #endregion PUT

        #region DELETE

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400, Type = typeof(string))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Delete(Guid id)
        {
            if(id.Equals(Guid.Empty))
                return BadRequest(CommonResponseMessages.NoId);

            if(!_userRepository.Exists(id))
                return NoContent();

            await _userRepository.Delete(id);
            return Ok();
        }

        #endregion DELETE
    }
}