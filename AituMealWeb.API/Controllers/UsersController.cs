using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AituMealWeb.Core.DTO.UserDTOs;
using AituMealWeb.Core.Entities;
using AituMealWeb.Core.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AituMealWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [Authorize(Roles = Role.Admin)]//Only admin can see all users
        [HttpGet]
        public async Task<IActionResult> FindAllUsersAsync()
        {
            var users = await _userRepository.GetAllRecs();
            var usersToReturn = _mapper.Map<IEnumerable<UserDetails>>(users);
            return users != null ? (IActionResult)Ok(usersToReturn) : NoContent();
        }

        [Authorize]
        [HttpGet("mypage/{id}")]
        public async Task<IActionResult> FindUser(Guid id)
        {
            var currentUser = HttpContext.User; //no one can see detail of other user except admin

            if (currentUser.HasClaim(u => u.Type != id.ToString())) {
                return BadRequest("Access denied! You cannot see User details of another User!");
            }

            var user = await _userRepository.GetRecById(id);
            var userToReturn = _mapper.Map<UserDetails>(user);
            return user != null ? (IActionResult)Ok(userToReturn) : NoContent();
        }

        //realize makeKassir operation
        [Authorize(Roles = Role.Admin)]
        [HttpPut("kassir/{mode:int}/{id}")]
        public async Task<IActionResult> UpgradeUserToKassir(int mode,Guid id)
        {
            var kassirKandidate = await _userRepository.GetRecById(id);

            if (kassirKandidate == null || kassirKandidate.Role == Role.Admin) //admin role cannot be changed to kassir
            {
                return BadRequest("Candidate user does not exist");
            }

            switch (mode) //in order to reduce code duplication, there were used mode parameter, which is used to choose make user or make kassir operations
            {
                case 1:
                    kassirKandidate.Role = Role.Kassir;
                    break;
                case 2:
                    kassirKandidate.Role = Role.User;
                    break;
                default:
                    return BadRequest("Only two operations avalable");
            }

            try
            {
                User newKassir = new User()
                {
                    Id = kassirKandidate.Id,
                    FirstName = kassirKandidate.FirstName,
                    LastName = kassirKandidate.LastName,
                    Email = kassirKandidate.Email,
                    Role = kassirKandidate.Role
                };

                await _userRepository.UpdateRec(kassirKandidate.Id, newKassir);
                return NoContent();
            }
            catch
            {
                return BadRequest("Some problems occured during Kassir submission process!");
            }
        }
    }
}
