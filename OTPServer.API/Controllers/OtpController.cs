using Microsoft.AspNetCore.Mvc;
using OtpNet.Api.Requests;
using OTPServer.API.Requests;
using OTPServer.Business.Dtos;
using OTPServer.Business.Services;
using System.ComponentModel.DataAnnotations;

namespace OTPServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : Controller
    {
        private readonly IOtpService _otpService;
        private readonly IUserService _userService;

        public OtpController(IUserService userService, IOtpService otpService)
        {
            _userService = userService;
            _otpService = otpService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> generate([FromBody] CreateUserRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || !(new EmailAddressAttribute().IsValid(request.Email)))
                {
                    return BadRequest(new { message = "A valid email address is required" });
                }

                var user = await _userService.GetByEmail(request.Email);
                if (user == null)
                {
                    user = await _userService.Create(new UserDto() { Email = request.Email });
                }
                var otp = await _otpService.GenerateOtp(user.Id);

                return Ok(otp);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error while generating otp." });
            }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> validate([FromBody] ValidateOtpRequest request)
        {
            try
            {

                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code) || !(new EmailAddressAttribute().IsValid(request.Email)))
                {
                    return BadRequest(new { message = "Please provide a valid email address and the one time password.", isValid = false });
                }
                var (isValid, message) = await _otpService.Validate(request.Email, request.Code);

                if (isValid)
                {
                    return Ok(new { message, isValid });
                }

                return BadRequest(new { message, isValid });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error while validating code", isValid = false });
            }
        }
    }
}
