using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN231_ProjectQA_Data.Entities;
using PRN231_ProjectQA_Service.Services;
using PRN231_ProjectQA_WebAPI.Models.User;
using static PRN231_ProjectQA_Data.Entities.User;

namespace PRN231_ProjectQA_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        public UserController(IMapper mapper, UserService userService)
        {
            _userService = userService;
            _mapper = mapper;

        }

        //Get all users
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                var userListModels = users.Select(u => _mapper.Map<UserModel>(u)).ToList();

                return Ok(userListModels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //Get user by id
        [HttpGet("{id}")]
        //[Authorize]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserById(id);
            if (user != null)
            {
                var userModel = _mapper.Map<UserModel>(user);
                return userModel == null ? NotFound() : Ok(userModel);
            }
            else
            {
                return NotFound();
            }

        }
        [HttpGet("profile/{id}")]
        //[Authorize]
        public async Task<IActionResult> GetProfileUser(Guid id)
        {
            var user = await _userService.GetUserById(id);
            if (user != null)
            {
                ProfileModel profileModel = new ProfileModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Img = user.Img,
                    QAnswered = user.Posts.Count,
                    QAsked = user.Comments.Count,   

                };
                return Ok(profileModel);
            }
            else
            {
                return NotFound();
            }

        }
        // Change status User
        [HttpPut("{id:guid}/{status:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserStatus(Guid id, UserStatus status)
        {
            try
            {
                await _userService.UpdateUserStatus(id, status);
                return Ok(new { message = $"User with id {id} updated successfully!" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating user with id {id}: {ex.Message}" });
            }
        }
        //// Update User
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = _mapper.Map<User>(userModel);
            user.Id = id;
            var roleID = await _userService.GetRoleIdByName(userModel.RoleName) ?? throw new ArgumentException("Invalid RoleName: Role does not exist.");
            user.RoleId = roleID;

            await _userService.UpdateUser(id, user);
            return Ok(new { message = $"User with id {id} updated successfully!" });
        }

        //Add new user
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewUser(AddUserModel userModel)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            try
            {
                var user = _mapper.Map<User>(userModel);
                var roleID = await _userService.GetRoleIdByName(userModel.RoleName);

                if (roleID == null) return BadRequest("Role dose not exists");

                else user.RoleId = roleID.Value;



                await _userService.AddNewUser(user);
                return Ok(user);
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("Email already exists"))
                {
                    return BadRequest("Email already exists");
                }

                else
                {

                    return StatusCode(500, "An error occurred while processing your request");
                }
            }

        }


    }
}
