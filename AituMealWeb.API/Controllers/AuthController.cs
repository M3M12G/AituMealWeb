using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AituMealWeb.Core.DTO.UserDTOs;
using AituMealWeb.Core.Entities;
using AituMealWeb.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace apiSandBox.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        [HttpPost("register")]//for regular users
        public async Task<IActionResult> RegisterAsync(RegisterModel userDetail)
        {
            if (!ModelState.IsValid)           //Validation mechanism invoke
                return BadRequest(ModelState);

            if (_authRepository.UserExists(userDetail.Email))
            {
                return BadRequest("This email is already exists!");
            }

            var userToCreate = new User
            {
                Id = Guid.NewGuid(),
                Email = userDetail.Email,
                FirstName = userDetail.FirstName,
                LastName = userDetail.LastName,
                Role = Role.User
            };

            var createdUser = await _authRepository.Register(userToCreate, userDetail.Password);
            if (createdUser == null)
            {
                return BadRequest("Problems with User registration!");
            }
            else {
                return StatusCode(201);//successfully registered
            }
        }

        [HttpPost("register-admin")]//for regular users
        public async Task<IActionResult> RegisterAdmin(RegisterModel userDetail)
        {
            if (!ModelState.IsValid)           //Validation mechanism invoke
                return BadRequest(ModelState); 

            if (_authRepository.UserExists(userDetail.Email))
            {
                return BadRequest("This email is already exists!");
            }

            var userToCreate = new User
            {
                Id = Guid.NewGuid(),
                Email = userDetail.Email,
                FirstName = userDetail.FirstName,
                LastName = userDetail.LastName,
                Role = Role.Admin
            };

            var createdUser = await _authRepository.Register(userToCreate, userDetail.Password);
            if (createdUser == null)
            {
                return BadRequest("Problems with User registration!");
            }
            else
            {
                return StatusCode(201);//successfully registered
            }
        }

        [HttpPost("login")]//for everyone
        public async Task<IActionResult> Login(LoginModel userDetail)
        {
            if (!ModelState.IsValid)           //Validation mechanism invoke
                return BadRequest(ModelState);

            var existingUser = await _authRepository.Login(userDetail.Email, userDetail.Password);
            
            if (existingUser == null)
                return Unauthorized("Incorrect Details");
            

                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
                new Claim(ClaimTypes.Name, existingUser.Email),
                new Claim(ClaimTypes.Role, existingUser.Role)
            };

                SymmetricSecurityKey key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)
                );

                SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddHours(2),//after 30 mins, token become invalid
                    SigningCredentials = creds
                };

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

                var accessToken = tokenHandler.WriteToken(token);
                return Ok(accessToken);
        }

        //need logout option. maybe realized on jquery, at client side
    }
}
