using Microsoft.AspNetCore.Mvc;
using PRN231_ProjectQA_MVC.Models.User;
using PRN231_ProjectQA_MVC.Models;
using System.Diagnostics;
using System.Text.Json;
using PRN231_ProjectQA_MVC.Authorizations;

namespace PRN231_ProjectQA_MVC.Controllers
{
    public class UserMController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public UserMController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("MyHttpClient");
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
        }
       
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Index(int? page, string searchString = null, string roleFilter = null, string statusFilter = null)
        {
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 10;
            List<UserModel> users = new List<UserModel>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/User");


            if (response.IsSuccessStatusCode)
            {
                string jsonString = response.Content.ReadAsStringAsync().Result;

                //convert from json to model
                users = JsonSerializer.Deserialize<List<UserModel>>
                    (jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            }
            else
            {
                users = new List<UserModel>(); // Or handle errors another way
            }
            // Search
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Username.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                         u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            // Filter by Role
            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = users.Where(u => u.RoleName.Equals(roleFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Filter by Status
            if (!string.IsNullOrEmpty(statusFilter))
            {
                users = users.Where(u => u.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.pageLength = (users.Count - 1) / pageSize + 1;
            users = users.Skip((int)((page - 1) * pageSize)).Take(pageSize).ToList();

            ViewBag.page = page;
            ViewBag.roleFilter = roleFilter;
            ViewBag.statusFilter = statusFilter;
            ViewBag.searchString = searchString;
            // Pass the data to the view
            return View(users);
        }

        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            UserModel user = new UserModel();
            if (TempData["UserData"] != null)
            {

                user = TempData["UserData"] as UserModel;
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            else
            {

                HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/User/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    string a = jsonString;
                    user = JsonSerializer.Deserialize<UserModel>(
                        jsonString,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }

                else
                {
                    user = new UserModel(); // Or handle errors another way
                }
            }

            return View(user);
        }
        [HttpGet]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create(Guid id)
        {
            return View();
        }

        [HttpPost]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Update(Guid id, string UserName, string Email, DateTime Dob, string Role)
        {
            var userModel = new UpdateUserModel
            {
                Username = UserName,
                Email = Email,
               
                RoleName = Role
            };

            try
            {
                var apiUrl = $"{_httpClient.BaseAddress}/User/{id}";
                var response = await _httpClient.PutAsJsonAsync(apiUrl, userModel);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Edit", new { id = id });
                }
                else
                {

                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Lỗi từ API: {errorContent}";
                    Console.WriteLine($"API call failed. Status code: {response.StatusCode}, Error: {errorContent}");

                    return RedirectToAction("Edit", new { id = id });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi hệ thống: {ex.Message}";
                TempData["UserData"] = userModel;
                return RedirectToAction("Edit", new { id = id });
            }
        }


        [HttpPost]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> ChangeStatus(Guid id, string status)
        {
            //Convert status to int
            status = status.Equals("Active", StringComparison.OrdinalIgnoreCase) ? "0" : "1";

            //Set URL
            var apiUrl = $"{_httpClient.BaseAddress}/User/{id}/{status}";

            //Call API
            HttpResponseMessage response = await _httpClient.PutAsync(apiUrl, null);


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new { id = id });
            }
            else
            {

                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Add(AddUserModel addUserModel)
        {


            if (!ModelState.IsValid)
            {

                return View(addUserModel);
            }

            try
            {
                //Call API
                var apiUrl = $"{_httpClient.BaseAddress}/User";
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiUrl, addUserModel);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    ModelState.AddModelError(string.Empty, $"Có lỗi xảy ra khi thêm người dùng. Mã lỗi: {response.StatusCode}. Chi tiết: {errorContent}");
                    return RedirectToAction("Create");

                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, "Lỗi hệ thống: " + ex.Message);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

    }
}
