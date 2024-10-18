using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PRN231_ProjectQA_MVC.Authorizations;
using PRN231_ProjectQA_MVC.Models.Comment;
using PRN231_ProjectQA_MVC.Models.Post;
using PRN231_ProjectQA_MVC.Models.Tag;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace PRN231_ProjectQA_MVC.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public PostController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("MyHttpClient");
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
        }
        public async Task<IActionResult> Index(int? page, string searchString = null)
        {

            //Get Posts
            List<PostModel> posts = new List<PostModel>();
            var apiUrl = $"{_httpClient.BaseAddress}/Post";
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                 posts = JsonConvert.DeserializeObject<List<PostModel>>(jsonResponse);

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
                ViewBag.page = page;

                var userId = GetUserIdToken();
                ViewBag.userId = userId;
            }
           

            return View(posts);
        }
        [HttpGet]
        public async Task<IActionResult> Ask()
        {
            
                return View();
        }
        [HttpPost]
        [AuthorizeToken]
        public async Task<IActionResult> Ask(string title, string content1, IFormFile img1, string content2, IFormFile img2)
        {
            // Ensure the directory exists
            var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }

            // Process the uploaded files
            string img1Path = null;
            string img2Path = null;

            // Process the first image
            if (img1 != null && img1.Length > 0)
            {
                try
                {
                    // Generate a unique file name
                    var img1FileName = Guid.NewGuid() + Path.GetExtension(img1.FileName);
                    // Combine the current directory with the target path
                    var img1PathToSave = Path.Combine(imagesDirectory, img1FileName);

                    // Create a file stream and copy the file content to the target path
                    using (var stream = new FileStream(img1PathToSave, FileMode.Create))
                    {
                        await img1.CopyToAsync(stream);
                    }
                    // Store the relative path for later use
                    img1Path = "/images/" + img1FileName;
                }
                catch (Exception ex)
                {
                    // Log the exception (you can use a logging framework)
                    Console.WriteLine($"Error uploading img1: {ex.Message}");
                }
            }

            // Process the second image
            if (img2 != null && img2.Length > 0)
            {
                try
                {
                    // Generate a unique file name
                    var img2FileName = Guid.NewGuid() + Path.GetExtension(img2.FileName);
                    // Combine the current directory with the target path
                    var img2PathToSave = Path.Combine(imagesDirectory, img2FileName);

                    // Create a file stream and copy the file content to the target path
                    using (var stream = new FileStream(img2PathToSave, FileMode.Create))
                    {
                        await img2.CopyToAsync(stream);
                    }
                    // Store the relative path for later use
                    img2Path = "/images/" + img2FileName;
                }
                catch (Exception ex)
                {
                    // Log the exception (you can use a logging framework)
                    Console.WriteLine($"Error uploading img2: {ex.Message}");
                }
            }

            // Get the user ID from the token
            var token = Request.Cookies["AuthToken"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;

            // Create a new post model
            var post = new AddPostModel
            {
                Title = title,
                Content1 = content1,
                Content2 = content2,
                Img1 = img1Path,
                Img2 = img2Path,
                UserId = Guid.Parse(userIdClaim),
            };

            // Send the post data to the API
            var response = await _httpClient.PostAsJsonAsync($"{_httpClient.BaseAddress}/Post", post);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }



        [HttpGet]
        public async Task<IActionResult> Details(Guid id, int? page)
        {
            PostDetailsModel post = new PostDetailsModel();   
            List<CommentModel> commentModels   = new List<CommentModel>();

            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Post/{id}");  
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                 post = JsonConvert.DeserializeObject<PostDetailsModel>(jsonResponse);
               
            }

            var response2 = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Comment/{id}");
            if (response2.IsSuccessStatusCode)
            {
                var jsonResponse = await response2.Content.ReadAsStringAsync();
                commentModels = JsonConvert.DeserializeObject<List<CommentModel>>(jsonResponse);
               
            }
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 10;
            ViewBag.pageLength = (commentModels.Count - 1) / pageSize + 1;
            commentModels = commentModels.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToList();
            ViewBag.page = page;

            ViewBag.Comments = commentModels;
            var userId = GetUserIdToken();
            ViewBag.userId = userId;
            return View(post);
        }
        
        
        [HttpPost]
        [AuthorizeToken]
        public async Task<IActionResult> Answer(string content, Guid? postId)
        {
            var AddComment = new AddCommentModel
            {
                Content = content,
                PostId = postId,
                CreatedAt = DateTime.Now,
                UserId = Guid.Parse(GetUserIdToken())
            };  
            var response = await _httpClient.PostAsJsonAsync($"{_httpClient.BaseAddress}/Comment", AddComment);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", "Post", new { id = postId });
            }
            return View();
        }
        [HttpPost]
        [AuthorizeToken]
        public async Task<IActionResult> Comment(Guid commentId, string content,Guid postId)
        {
            var AddComment = new AddAnswerCommentmodel
            {

                Content = content,
                CommentId = commentId,
                CreatedAt = DateTime.Now,
                UserId = Guid.Parse(GetUserIdToken())
            };  
            var response = await _httpClient.PostAsJsonAsync($"{_httpClient.BaseAddress}/Comment/AnswerComment", AddComment);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", "Post" ,new {id = postId });
            }
            return View();
        }
        [HttpGet]
        [AuthorizeToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Post/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        [AuthorizeToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            PostDetailsModel post = new PostDetailsModel();
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Post/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                post = JsonConvert.DeserializeObject<PostDetailsModel>(jsonResponse);
            }

          

            return View(post);
        }
        [HttpPost]
        [AuthorizeToken]
        public async Task<IActionResult> Edit(Guid id, string img1, string img2, string content1, string content2, string title)
        {
            var post = new AddPostModel
            {
               
                Title = title,
                Content1 = content1,
                Content2 = content2,
                Img1 = img1,
                Img2 = img2
            };
        
        var response = await _httpClient.PutAsJsonAsync($"{_httpClient.BaseAddress}/Post/{id}", post);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", "Post", new { id = id });
            }
            return View();
        }
           
        private string GetUserIdToken()
        {
            var token = Request.Cookies["AuthToken"];
            if (token == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "userId")?.Value;
            return userIdClaim;

        }


    }
}
