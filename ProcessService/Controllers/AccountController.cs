using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProcessService.Entities;
using ProcessService.Interfaces;
using DTO;
using System.Web;
using ProcessService.Mail;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email };
            var profile = new Profile
            {
                Id = Guid.NewGuid().ToString()
            };
            user.Profile = profile;

            if (vm.Password == vm.ConfirmPassword)
            {
                var result = await _userManager.CreateAsync(user, vm.Password);

                if (result.Succeeded)
                {
                    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    confirmationToken = HttpUtility.UrlEncode(confirmationToken);

                    //Send mail with confirnmationlink
                    var mailManager = new MailManager(_config);
                    await mailManager.SendRegistrationMessageAsync(user.Email, user.Id, confirmationToken);
                }
                else
                    return GetErrorResult(result);
            }
            else
            {
                return BadRequest();
            }

            await _userManager.AddToRoleAsync(user, "Consumer");
            return Ok();
        }


        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(ConfirmEmailViewModel vm)
        {
            var code = HttpUtility.UrlDecode(vm.Code);

            var user = await _userManager.FindByIdAsync(vm.Id);
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                var error = new ProcessServiceError
                {
                    Error = ServiceErrorCode.ConfirmAccountFailed,
                    ErrorDescription = "Could not confirm your account"
                };
                return BadRequest(error);
            }

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RegisterAccountForRestaurant(RegisterRestaurantViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var restaurantOwner = new ApplicationUser { UserName = vm.Email, Email = vm.Email };
            var profile = new Profile
            {
                Id = Guid.NewGuid().ToString(),
                Name = vm.Name
            };

            restaurantOwner.Profile = profile;
            var result = await _userManager.CreateAsync(restaurantOwner);

            if (result.Succeeded)
            {
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(restaurantOwner);
                confirmationToken = HttpUtility.UrlEncode(confirmationToken);

                //Send mail with confirnmationlink
                var mailManager = new MailManager(_config);
                await mailManager.SendRegistrationMessageAsync(restaurantOwner.Email, restaurantOwner.Id, confirmationToken);
            }
            else
                return GetErrorResult(result);

            await _userManager.AddToRoleAsync(restaurantOwner, "RestaurantOwner");

            return CreatedAtAction(nameof(RegisterAccountForRestaurant), new { id = restaurantOwner.Id }, restaurantOwner);
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmRestaurantOwnerEmail(ConfirmRestaurantOwnerViewModel vm)
        {
            var code = HttpUtility.UrlDecode(vm.Code);

            var restaurantOwner = await _userManager.FindByIdAsync(vm.Id);
            var result = await _userManager.ConfirmEmailAsync(restaurantOwner, code);

            if(!result.Succeeded)
            {
                var error = new ProcessServiceError
                {
                    Error = ServiceErrorCode.ConfirmAccountFailed,
                    ErrorDescription = "Could not confirm your account"
                };
                return BadRequest(error);
            }

            var passwordResult = await _userManager.AddPasswordAsync(restaurantOwner, vm.Password);

            if (passwordResult.Succeeded)
                return Ok();
            else
            {
                var error = new ProcessServiceError
                {
                    Error = ServiceErrorCode.SetPasswordFailed,
                    ErrorDescription = "Could not set password"
                };
                return BadRequest(error);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(UserId);
            var result = await _userManager.AddPasswordAsync(user, vm.Password);

            if (!result.Succeeded)
                return GetErrorResult(result);

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            var restaurantOwner = await _userManager.FindByEmailAsync(vm.Email);

            if (User == null || (await _userManager.IsEmailConfirmedAsync(restaurantOwner)))
                return BadRequest();

            var profile = await _uow.ProfileRepository.FindByCondition(p => p.Id == restaurantOwner.ProfileId).FirstOrDefaultAsync();

            string code = await _userManager.GeneratePasswordResetTokenAsync(restaurantOwner);
            var mailManager = new MailManager(_config);

            var encodedToken = HttpUtility.UrlEncode(code);
            await mailManager.SendForgotPasswordMessageAsync(restaurantOwner.Email, encodedToken);

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (ModelState == null || !ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(vm.Email);

            if (user == null)
                return NotFound();

            var result = await _userManager.ResetPasswordAsync(user, vm.Token, vm.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.FirstOrDefault());

            return Ok();

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(UserId);
            var result = await _userManager.ChangePasswordAsync(user, vm.CurrentPassword, vm.NewPassword);

            if (!result.Succeeded)
                return GetErrorResult(result);

            return Ok();
        }

        #region private methods

        private ActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
                return StatusCode(500);

            string errorString = string.Empty;

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        errorString += error + " ";
                    }
                }
                if (ModelState.IsValid)
                    return BadRequest();

                var serviceError = new ProcessServiceError
                {
                    Error = ServiceErrorCode.GeneralError,
                    ErrorDescription = errorString
                };

                return BadRequest(serviceError);
            }

            return null;
        }

        #endregion
    }
}
