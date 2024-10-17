using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PRN231_ProjectQA_Data.Entities;
using PRN231_ProjectQA_Service.Services;
using PRN231_ProjectQA_WebAPI.Models.Post;

namespace PRN231_ProjectQA_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly PostService _postService;
        public PostController(IMapper mapper, PostService postService)
        {
            _mapper = mapper;
            _postService = postService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var posts = await _postService.GetAllPosts();
                var postListModels = new List<PostModel>();
                foreach (var post in posts)
                {
                    PostModel postListModel = _mapper.Map<PostModel>(post);
                    postListModel.Username = post.User.Username;
                    postListModel.UserImg = post.User.Img;


                    postListModel.TotalComment = postListModels.Count;
                    postListModels.Add(postListModel);
                }

                return Ok(postListModels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostDetails(Guid id)
        {
            try
            {
                var post = await _postService.GetPostById(id);
               
                if (post == null)
                {
                    return NotFound();
                }
                var postdetail = _mapper.Map<PostDetailsModel>(post);
                postdetail.TotalComment = post.Comments.Count;
                return Ok(postdetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPost([FromBody] AddPostModel addPostModel)
        {
            try
            {
                var post = _mapper.Map<Post>(addPostModel);

                await _postService.AddNewPost(post);
                return Ok("Add Successfull !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


        }

        [HttpGet("getbyuserId/{userId}")]
        public async Task<IActionResult> GetPostsByUser(Guid userId)
        {
            try
            {

                var posts = await _postService.GetPostsByUser(userId);
                List<PostDetailsModel> postDetailsModels = new List<PostDetailsModel>();
                foreach (var post in posts)
                {
                    PostDetailsModel postDetailsModel = _mapper.Map<PostDetailsModel>(post);
                    postDetailsModel.TotalComment = post.Comments.Count;
                    postDetailsModels.Add(postDetailsModel);
                }

                return Ok(postDetailsModels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }



        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletePost(Guid id)
        //{
        //    try
        //    {
        //        await _postService.DeletePost(id);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}  









    }
}
