
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRN231_ProjectQA_MVC.Authorizations;
using PRN231_ProjectQA_MVC.Models.Authen;
using PRN231_ProjectQA_MVC.Models.Post;
using PRN231_ProjectQA_MVC.Models.User;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.Json;

namespace PRN231_ProjectQA_MVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
      
        public  ProfileController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("MyHttpClient");
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
    }
        [HttpGet]
        [AuthorizeToken]
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            // Gọi API để lấy thông tin người dùng
            var response = await _httpClient.GetAsync($"/api/User/profile/{userIdClaim}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<ProfileModel>(jsonResponse);

            var responsePost = await _httpClient.GetAsync($"/api/Post/getbyuserId/{userIdClaim}");
            if (!responsePost.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var jsonResponsePost = await responsePost.Content.ReadAsStringAsync();
            var posts = JsonConvert.DeserializeObject<List<PostDetailsModel>>(jsonResponsePost);

            ViewBag.Posts = posts;

            return View(user);
           
        }

        [HttpGet]
        [AuthorizeToken]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [AuthorizeToken]
        public async Task<IActionResult> ChangePassword(string NewPassword, string OldPassword)
        {
            var token = Request.Cookies["AuthToken"];
            ChangePassModel changePassModel = new ChangePassModel
            {
                Token = token,
                OldPassword = OldPassword,
                NewPassword = NewPassword
            };

            // API login URLAuth/ChangePassword
            var apiUrl = $"{_httpClient.BaseAddress}/Auth/ChangePassword";

            // Send a POST request to the API
            var response = await _httpClient.PostAsJsonAsync(apiUrl, changePassModel);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Your password has been changed successfully.";
                return View();
            }
            else
            {
                // Read the error message from the API response
                string errorMessage = await GetResponeMessageAPI(response);

                // Pass the error message to the view
                ViewBag.ErrorMessage = errorMessage;
                return View(); // Return to the same view
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
