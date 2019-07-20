using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        public async Task<ActionResult> GetAllAccounts()
        {
            var results = await _userAssembler.GetUserAccounts();
            return Ok(results);
        }



    }
}