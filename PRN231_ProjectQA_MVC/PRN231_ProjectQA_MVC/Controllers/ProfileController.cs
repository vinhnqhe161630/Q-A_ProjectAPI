
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRN231_ProjectQA_MVC.Authorizations;
using PRN231_ProjectQA_MVC.Models.Post;
using PRN231_ProjectQA_MVC.Models.User;
using System.IdentityModel.Tokens.Jwt;

namespace PRN231_ProjectQA_MVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
      
        public  ProfileController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
    }
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
    }
}
