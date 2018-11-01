using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AP.Cryptography;
using Microsoft.Extensions.Configuration;
using CacheManager.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models = AP.Entities.Models;
using AP.Entities.Options;
using AP.Repositories.User;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace AP.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICacheManager<string> _cache;
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository,IConfiguration configuration, ICacheManager<string> cache)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _cache = cache;
        }

        [HttpGet("{username}")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetChallange([Required] string username)
        {
            if(string.IsNullOrWhiteSpace(username))
                return BadRequest("EMPTY_USERNAME");

            string salt = Challange(username);

            return Ok(salt); 
        }

        [HttpPost("{username}")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Authenticate(string username, [FromBody] string password)
        {
            if(string.IsNullOrWhiteSpace(username))
                return BadRequest("EMPTY_USERNAME");
            else if(string.IsNullOrWhiteSpace(password))
                return BadRequest("EMPTY_PASSWORD");

            var credentials = new Credentials
            {
                Username = username,
                Password = password
            };

            string token = Authenticate(credentials);

            if(string.IsNullOrWhiteSpace(token))
                return Unauthorized();

            return Ok(token);
        }

        #region Private

        private string Challange(string username)
        {
            if(_cache.Exists(username))
                return _cache.Get(username);

            var rng = new RNG();

            string salt = rng.GenerateRandomCryptographicKey(64);
            CacheItem<string> cacheItem = new CacheItem<string>(username, salt, ExpirationMode.Absolute, TimeSpan.FromMinutes(5));
            _cache.Add(cacheItem);

            return salt;
        }

        private string BuildToken(Models.User user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(Double.Parse(_configuration["Jwt:TokenExpire"])),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string Authenticate(Credentials login)
        {
            if (String.IsNullOrWhiteSpace(login.Username) || String.IsNullOrWhiteSpace(login.Password))
                return null;

            var salt = _cache.Get(login.Username);
            if (String.IsNullOrWhiteSpace(salt))
                return null;

            var user = _userRepository.GetUserByUsername(login.Username).Result;
            if (user == null)
                return null;

            string saltedPassword = SHA.ComputePasswordAndSalt(user.Password, salt, SHA256.Create());

            return saltedPassword.Equals(login.Password) ? BuildToken(user) : null;
        }

        #endregion Private
    }
}