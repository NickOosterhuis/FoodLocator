using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProcessService.Entities;
using ProcessService.Interfaces;

namespace ProcessService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IUoW _uow;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;

        public string UserId => User.FindFirst(ClaimTypes.Sid).Value;

        public AccountController(IUoW uow, IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _config = config;
            _userManager = userManager;
        }
    }
}
