using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProcessService.Entities;
using DTO;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProcessService.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Produces("application/json")]
    public class TokenController : ControllerBase
    { 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        
        public TokenController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }

        [HttpPost]
        [AllowAnonymous]        
        public async Task<ActionResult<TokenResult>> Login(LoginViewModel vm)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);

                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, vm.Password, false);

                    if (result.Succeeded)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Sid, user.Id)
                        };

                        var userRoles = await _userManager.GetRolesAsync(user);

                        foreach (var userRole in userRoles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, userRole));
                            var role = await _roleManager.FindByNameAsync(userRole);
                            if (role != null)
                            {
                                var roleClaims = await _roleManager.GetClaimsAsync(role);
                                foreach (Claim roleClaim in roleClaims)
                                {
                                    claims.Add(roleClaim);
                                }
                            }
                        }

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AppSettings:Jwt:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var expires = DateTime.Now.AddDays(Convert.ToDouble(_config["AppSettings:Jwt:ExpireDays"]));

                        var token = new JwtSecurityToken(
                            _config["AppSettings:Jwt:Issuer"],
                            _config["AppSettings:Jwt:Issuer"],
                            claims,
                            expires: expires,
                            signingCredentials: creds
                        );

                        var tokenResult = new TokenResult
                        {
                            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                            Email = user.Email,
                            Expires = expires,
                            UserName = user.UserName,
                            Role = string.Join(",", userRoles),
                            ExpiresIn = 1,
                            Id = user.Id,
                        };

                        return tokenResult;
                    }
                }

                var error = new ProcessServiceError
                {
                    Error = ServiceErrorCode.WrongUserOrPassword,
                    ErrorDescription = "Wrong username or password"
                };
                return BadRequest(error);
            }
            catch (Exception e)
            {
                var error = new ProcessServiceError
                {
                    Error = ServiceErrorCode.GeneralError,
                    ErrorDescription = "Something went wrong"
                };
                return BadRequest(error);
            }
        }
    }
}
