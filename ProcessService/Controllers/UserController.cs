using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProcessService.Assemblers;
using ProcessService.Entities;
using ProcessService.Interfaces;

namespace ProcessService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUoW _uow;
        private readonly UserAssembler _userAssembler;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationUser> _roleManager;

        public string UserId => User.FindFirst(ClaimTypes.Sid).Value;

        public UserController(IUoW uow, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _userManager = userManager;
            _userAssembler = new UserAssembler(uow, userManager);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllAccounts()
        {
            var results = await _userAssembler.GetUserAccounts();
            return Ok(results);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAccountById(string id)
        {
            var result = await _userAssembler.GetUserAccount(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult> GetOwnUserAccount()
        {
            var result = await _userAssembler.GetOwnProfile(UserId);
            return Ok(result);
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult> UpdateOwnUserAccount([FromBody] UserAccount userAccount)
        {
            try
            {
                var result = await _userAssembler.UpdateUserProfile(UserId, userAccount);
                return Accepted(result);
            }
            catch
            {
                var error = new ProcessServiceError
                {
                    Error = ServiceErrorCode.ErrorUpdatingUser,
                    ErrorDescription = "Could not update this user"
                };

                return BadRequest(error);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUserAccount(string id, [FromBody] UserAccount userAccount)
        {
            try
            {
                var result = await _userAssembler.UpdateUserProfile(id, userAccount);
                return Accepted(result);
            }
            catch
            {
                var error = new ProcessServiceError
                {
                    Error = ServiceErrorCode.ErrorUpdatingUser,
                    ErrorDescription = "Could not update this user"
                };

                return BadRequest(error);
            }
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize]
        public async Task<ActionResult> UpdateProfilePicture(string imageUrl)
        {
            try
            {
                var result = await _userAssembler.UpdateProfilePicture(UserId, imageUrl);
                return Accepted(result);
            }
            catch
            {
                var error = new ProcessServiceError
                {
                    Error = ServiceErrorCode.ErrorUpdatingProfilePicture,
                    ErrorDescription = "Could not update the profile picture"
                };

                return BadRequest(error);
            }
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateRestaurantFeatured(string id, bool featured)
        {
            try
            {
                var result = await _userAssembler.UpdateRestaurantFeatured(id, featured);
                return Accepted(result);
            }
            catch
            {
                var error = new ProcessServiceError
                {
                    Error = ServiceErrorCode.ErrorUpdatingRestaurantFeatured,
                    ErrorDescription = "Could not update restaurant featured"
                };

                return BadRequest(error);
            }
        }





    }
}