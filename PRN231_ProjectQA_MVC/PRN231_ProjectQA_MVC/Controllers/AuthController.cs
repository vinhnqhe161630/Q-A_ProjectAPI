using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using PRN231_ProjectQA_MVC.Models.Authen;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

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
        public async Task<IActionResult> Login()
        {
            // get token from cookie
            var token = Request.Cookies["AuthToken"];


            if (!string.IsNullOrEmpty(token))
            {

                return RedirectToAction("HomeShop", "ShopProduct");
            }

            // get email from cookie if remember
            var remember = Request.Cookies["Remember"];
            ViewBag.Remember = remember;

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
                    return RedirectToAction("Index", "User");
                }

                // Redirect to the home page or the desired page after a successful login
                return RedirectToAction("HomeShop", "ShopProduct");
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
        private string HandleSuccessfulLogin(string responseContent)
        {
            // Parse JSON to get the token
            var json = JObject.Parse(responseContent);
            string token = json["token"].ToString();


            // Save the token in a cookie
            CookieOptions cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Allow access to the cookie only through HTTP (more secure)
                Expires = DateTime.Now.AddDays(3) // Set the expiration time for the cookie
            };

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

    }
}
