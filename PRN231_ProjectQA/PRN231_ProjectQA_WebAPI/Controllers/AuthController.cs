using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN231_ProjectQA_Data.Entities;
using PRN231_ProjectQA_Service.Services;
using PRN231_ProjectQA_WebAPI.Models.Auth;

namespace PRN231_ProjectQA_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(AuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LogIn(LoginModel model)
        {
            IActionResult response;

            //InValid Model
            if (!ModelState.IsValid)
            {
                response = BadRequest();
            }
            //mapper loginmodel to user
            var user = _mapper.Map<User>(model);

            //Check acc and create token
            var token = await _authService.LoginAsync(user);

            //Invalid account and returned emtry
            if (string.IsNullOrEmpty(token))
            {
                response = Unauthorized(new { message = "Either email address or password is incorrect. Please try again" });
            }
            else if (token.Equals("Inactive"))
            {
                response = Unauthorized(new { message = "Your account is disabled. Contact us for help." });
            }
            else
            {
                response = Ok(new { token });
            }

            return response;
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                IActionResult response;

                var isUser = await _authService.IsUser(email);

                if (!isUser)
                {
                    response = NotFound(new { message = "Email not found" });
                }
                else
                {

                    var token = _authService.GenerateToken(email);

                    _authService.SendResetPasswordEmail(email, token);
                    response = Ok(new { message = "Email sent" });
                }

                return response;
            }
            catch (Exception ex)
            {
                // Handle potential errors
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                // Validate model
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid password reset request" });

                // Check for null values in model properties
                if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Token))
                    return BadRequest(new { message = "Password and token cannot be empty" });

                User user = _mapper.Map<User>(model);

                // Attempt to reset password
                await _authService.ResetPassword(user);

                return Ok(new { message = "Password reset successfully" });
            }
            catch (Exception ex)
            {
                // Handle potential errors
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            try
            {

                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid change password request" });

                //Check old and new pass are null
                if (string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.Token))
                    return BadRequest(new { message = "Password and token cannot be empty" });
                await _authService.ChangePasswordAsync(model.Token, model.NewPassword, model.OldPassword);

                return Ok(new { message = "Password Changed successfully" });
            }
            catch (Exception ex)
            {
                // Handle potential errors
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("Login_Google")]
        public async Task<IActionResult> Login_Google(LoginGoogleModel loginGoogle)
        {
            try
            {
                IActionResult response;
                // Check if the account exists using email and Google ID
                var token = await _authService.Login_GoogleAsync(loginGoogle.Email, loginGoogle.GoogleId);

                if (string.IsNullOrEmpty(token))
                {
                    // Map the LoginGoogleModel to a User entity
                    var user = _mapper.Map<User>(loginGoogle);
                    user.Password = "1@113$2aMGs";
                    // Create a new account
                    token = await _authService.CreateNewAccount(user);
                }

                // Return the generated token
                response = Ok(new { token });
                return response;
            }
            catch (Exception ex)
            {

                // Return a 500 Internal Server Error with a custom error message
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpModel signUpModel)
        {
            try
            {
                IActionResult response;
                var user = _mapper.Map<User>(signUpModel);
                user.Img = "./assets/images/boy.png";
                var token = await _authService.CreateNewAccount(user);
                response = Ok(new { token });
                return response;
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }


    }



}
