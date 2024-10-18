using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using PRN231_ProjectQA_MVC.Models.Authen;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using PRN231_ProjectQA_MVC.Authorizations;

namespace PRN231_ProjectQA_MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        public IActionResult Index()
        {
            return View();
        }
        public AuthController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
        }

        [HttpGet]
        public IActionResult LoginWithGoogle()
        {

            var properties = new AuthenticationProperties
            {

                RedirectUri = Url.Action("GoogleResponse", "Auth", null, Request.Scheme)
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return BadRequest("Failed to authenticate with Google.");
            }
            var claims = result.Principal.Claims.ToList();
            // Lấy các giá trị cụ thể từ các claim theo loại
            var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var pictureUrl = claims.FirstOrDefault(c => c.Type == "picture")?.Value;
            // Lấy thông tin từ claims


            LoginGoogleModel googleModel = new LoginGoogleModel
            {
                GoogleId = googleId,
                Img = pictureUrl,
                Email = email,
                Username = name,
            };

            // Construct the API URL if BaseAddress is already set in _httpClient
            var apiUrl = $"{_httpClient.BaseAddress}/Auth/Login_Google";

            // Send the request to the API
            var response = await _httpClient.PostAsJsonAsync(apiUrl, googleModel);

            var responseContent = await response.Content.ReadAsStringAsync();

            // If the login is successful
            if (response.IsSuccessStatusCode)
            {
                var role = HandleSuccessfulLogin(responseContent);


                if (role == "Admin")
                {
                    return RedirectToAction("Index", "UserM");
                }

                // Redirect to the home page or the desired page after a successful login
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // If login fails, redirect to the login page with an error message
                ViewBag.ErrorMessage = "Login failed. Please try again.";
                return View("Login");
            }

        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            // get token from cookie
            var token = Request.Cookies["AuthToken"];


            if (!string.IsNullOrEmpty(token))
            {

                return RedirectToAction("Index", "Home");
            }


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            // Create a LoginModel object with email and password from the form
            LoginModel loginModel = new LoginModel
            {
                Email = email,
                Password = password
            };

            // API login URL
            var apiUrl = $"{_httpClient.BaseAddress}/Auth/Login";

            // Send a POST request to the API
            var response = await _httpClient.PostAsJsonAsync(apiUrl, loginModel);

            var responseContent = await response.Content.ReadAsStringAsync();
            // If the login is successful
            if (response.IsSuccessStatusCode)
            {
                // Call HandleSuccessfulLogin and get the role
                var role = HandleSuccessfulLogin(responseContent);


                if (role == "Admin")
                {
                    return RedirectToAction("Index", "UserM");
                }

                // Redirect to the home page or the desired page after a successful login
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Read the error message from the API response
                string errorMessage = await GetResponeMessageAPI(response);

                // Pass the error message to the view
                ViewBag.ErrorMessage = errorMessage;
                return View("Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> ForgotPassword(string Email)
        {
            var apiUrl = $"{_httpClient.BaseAddress}/Auth/ForgotPassword?email=" + Email;

            // Create HttpContent with the email


            // Send POST request to the API
            var response = await _httpClient.PostAsync(apiUrl, null);
            if (response.IsSuccessStatusCode)
            {

                ViewBag.SuccessMessage = "Your email has been sent successfully. Please check your inbox for further instructions.";
                return View();
            }
            else
            {
                // Read the error message from the API response
                string errorMessage = await GetResponeMessageAPI(response);

                // Pass the error message to the view
                ViewBag.ErrorMessage = errorMessage;
                return View();
            }

        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, string password)
        {
            var apiUrl = $"{_httpClient.BaseAddress}/Auth/ResetPassword";


            ResetPassModel resetPass = new ResetPassModel
            {
                Token = token,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, resetPass);

            if (response.IsSuccessStatusCode)
            {
                string message = await GetResponeMessageAPI(response);
                ViewBag.SuccessMessage = message;
                return View();
            }
            else
            {
                // Read the error message from the API response
                string errorMessage = await GetResponeMessageAPI(response);

                // Pass the error message to the view
                ViewBag.ErrorMessage = errorMessage;
                return View();
            }


        }
        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            var token = Request.Cookies["AuthToken"];


            if (!string.IsNullOrEmpty(token))
            {

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(string email, string username, string password)
        {
            SignUpModel signUpModel = new SignUpModel
            {
                Email = email,
                Username = username,
                Password = password
            };

            var apiUrl = $"{_httpClient.BaseAddress}/Auth/SignUp";

            var response = await _httpClient.PostAsJsonAsync(apiUrl, signUpModel);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                HandleSuccessfulLogin(responseContent);


                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Read the error message from the API response
                string errorMessage = await GetResponeMessageAPI(response);

                // Pass the error message to the view
                ViewBag.ErrorMessage = errorMessage;
                return View();
            }

        }
        private string HandleSuccessfulLogin(string responseContent)
        {
            // Save the token in a cookie
            CookieOptions cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Allow access to the cookie only through HTTP (more secure)
                Expires = DateTime.Now.AddDays(3) // Set the expiration time for the cookie
            };

            // Parse JSON to get the token
            var json = JObject.Parse(responseContent);

            string toke_Img = json["token"].ToString();
            string[] split = toke_Img.Split(" ");   
            string token = split[0];
            if (split.Length > 1) {
                Response.Cookies.Append("Img", split[1], cookieOptions);
            }

           
            Response.Cookies.Append("AuthToken", token, cookieOptions);

            var handler = new JwtSecurityTokenHandler();
            try
            {
                //Get role form Token
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                var role = roleClaim?.Value;

              
              


                return role;
            }
            catch (SecurityTokenException ex)
            {
                // Log or handle token parsing exception
                Console.WriteLine($"Token parsing error: {ex.Message}");

                return null; // Return null if an error occurs
            }

        }
        private async Task<string> GetResponeMessageAPI(HttpResponseMessage response)
        {
            // Read the error message from the API response
            string message = "An error occurred while changing the password.";

            // Read the response content
            var responseContent = await response.Content.ReadAsStringAsync();

            // Parse the response content as JSON
            using (var document = JsonDocument.Parse(responseContent))
            {
                if (document.RootElement.TryGetProperty("message", out var messageElement))
                {
                    message = messageElement.GetString();
                }
            }
            return message;

        }

       
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Remove the AuthToken cookie
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("Img");
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
