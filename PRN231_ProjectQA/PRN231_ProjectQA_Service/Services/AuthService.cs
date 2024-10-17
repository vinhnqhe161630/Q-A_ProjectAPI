using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN231_ProjectQA_Data.Entities;
using PRN231_ProjectQA_Data.IRepositories;
using PRN231_ProjectQA_Service.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static PRN231_ProjectQA_Data.Entities.User;

namespace PRN231_ProjectQA_Service.Services
{
    public class AuthService
    {
        private readonly IAuthRepository authRepository;
        private readonly IUserRepository userRepository;
        private readonly EmailService emailService;
        private readonly IConfiguration _configuration;
        public AuthService(IAuthRepository authRepository, IConfiguration configuration, IUserRepository userRepository, EmailService emailService)
        {
            this.authRepository = authRepository;
            this.userRepository = userRepository;
            _configuration = configuration;
            this.emailService = emailService;
        }

        public async Task<string> LoginAsync(User userMapper)
        {

            //Check email and pass
            var user = await authRepository.IsValidUser(userMapper.Email, userMapper.Password);

            return await CreateToken(user);

        }
        public async Task<string> Login_GoogleAsync(string email, string googleID)
        {

            //Check email and googleID
            var user = await authRepository.Login_Google(email, googleID);

            return await CreateToken(user);

        }
        private async Task<string> CreateToken(User? user)
        {
            if (user == null) return string.Empty;
            if (user.Status == UserStatus.Inactive) return "Inactive";

            //add email to claim
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Email, user.Email),
                new("userId", user.Id.ToString()),
                new("username",user.Username),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //Get role of user
            var userRole = await authRepository.GetRoleUser(user.Email);
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));



            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

            var jwtService = new JwtService(_configuration["JWT:Secret"]!, _configuration["JWT:ValidIssuer"]!);
            ////Create token
            var token = jwtService.GenerateTokenLogin(authClaims);



            return token + " "+user.Img;
        }


        public async Task<bool> IsUser(string email)
        {
            return await authRepository.IsUser(email);
        }

        private async Task<string> ValidateToken(string token)
        {
            var jwtService = new JwtService(_configuration["JWT:Secret"]!, _configuration["JWT:ValidIssuer"]!);

            if (JwtService.IsTokenExpired(token))
            {
                throw new Exception("Token expired");
            }
            var principal = jwtService.GetPrincipal(token) ?? throw new Exception("Invalid token");
            var emailClaim = principal.FindFirst(ClaimTypes.Email) ?? throw new Exception("Email claim not found in token");

            var email = emailClaim.Value;

            var tokenFromDb = await authRepository.GetToken(email) ?? throw new Exception("Token not found");

            if (tokenFromDb != token)
            {
                throw new Exception("Invalid token");
            }

            return email;
        }


        public async Task ResetPassword(User user)
        {
            // Validate token
            var token = user.Token ?? throw new Exception("Token not found");
            try
            {
                var email = await ValidateToken(token);
                user.Email = email;
                // If token is valid, proceed with password reset
                await authRepository.ResetPassword(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string GenerateToken(string email)
        {
            var jwtService = new JwtService(_configuration["JWT:Secret"]!, _configuration["JWT:ValidIssuer"]!);
            var token = jwtService.GenerateToken(email);
            authRepository.SetToken(email, token);
            return token;
        }

        public void SendResetPasswordEmail(string userEmail, string resetToken)
        {
            var resetUrl = $"https://localhost:7130/Auth/ResetPassword?token={resetToken}";

            var email = new EmailDto
            {
                To = userEmail,
                Subject = "Password Reset",
                Body = $"We have just received a password reset request for {userEmail}.<br><br>" +
                $"Please click <a href=\"{resetUrl}\">here</a> to reset your password.<br><br>" +
                $"For your security, the link will expire in 24 hours or immediately after you reset your password."
            };

            emailService.SendEmail(email);
        }

        public async Task ChangePasswordAsync(string token, string newPassword, string oldPassword)
        {

            try
            {
                //get email in token
                var jwtService = new JwtService(_configuration["JWT:Secret"]!, _configuration["JWT:ValidIssuer"]!);

                if (JwtService.IsTokenExpired(token))
                {
                    throw new Exception("Token expired");
                }
                var principal = jwtService.GetPrincipal(token) ?? throw new Exception("Invalid token");
                var emailClaim = principal.FindFirst(ClaimTypes.Email) ?? throw new Exception("Email claim not found in token");

                var email = emailClaim.Value;


                //get user by email
                User user = await authRepository.GetUserByEmail(email);
                if (user == null) throw new Exception("User not found");

                //Check oldpass 
                user.Password = BCrypt.Net.BCrypt.Verify(oldPassword, user.Password) ? newPassword
                    ?? throw new ArgumentNullException(nameof(newPassword), "New password cannot be null.")
                    : throw new UnauthorizedAccessException("Old password is incorrect.");


                //Reset pass
                await authRepository.ResetPassword(user);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<string> CreateNewAccount(User user)
        {

            // Check if the user already exists
            var existingUser = await authRepository.GetUserByEmail(user.Email);
            if (existingUser == null)
            {
                // Set new user information
               
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                // Get RoleId for role "User"
                var roleId = await userRepository.GetRoleIdbyName("User");
                if (roleId.HasValue)
                {
                    user.RoleId = roleId.Value;
                }
                user.Status = UserStatus.Active;
                // SignUp new User
                await authRepository.SignUp(user);

                // Create and gen token
                return await CreateToken(user);
            }
            else
            {
                // Returns a message or value if the user already exists
                throw new InvalidOperationException("User already exists.");
            }
        }

    }




}
