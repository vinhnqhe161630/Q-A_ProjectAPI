using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PRN231_ProjectQA_MVC.Models;
using PRN231_ProjectQA_MVC.Models.Post;
using System.Diagnostics;

namespace PRN231_ProjectQA_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

       
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
       
        public HomeController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
        }
        public async Task<IActionResult> Index(int? page, string searchString = null)
        {
           

            var apiUrl = $"{_httpClient.BaseAddress}/Post";
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var posts = JsonConvert.DeserializeObject<List<PostModel>>(jsonResponse);

                //Search
                if (!string.IsNullOrEmpty(searchString))
                {
                    posts = posts.Where(s => s.Title.ToLower().Contains(searchString.ToLower())).ToList();
                    ViewBag.searchString = searchString;
                }
                    //Paging
                    if (page == null)
                {
                    page = 1;
                }
                int pageSize = 10;
                ViewBag.pageLength = (posts.Count - 1) / pageSize + 1;
                posts = posts.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToList();
                posts = posts.OrderByDescending(p => p.CreatedAt).ToList();
           

                ViewBag.page = page;
                return View(posts);

            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
