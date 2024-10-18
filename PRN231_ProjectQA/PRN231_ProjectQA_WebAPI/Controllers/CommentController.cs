using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN231_ProjectQA_Data.Entities;
using PRN231_ProjectQA_Service.Services;
using PRN231_ProjectQA_WebAPI.Models.Comment;

namespace PRN231_ProjectQA_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService commentService;
        private readonly IMapper _mapper;
        public CommentController(CommentService commentService, IMapper mapper)
        {
            this.commentService = commentService;
            _mapper = mapper;
        }
        
        [HttpGet("{postId}")]
        public async Task<ActionResult> GetCommentsAsync(Guid postId)
        {
            var comments = await commentService.GetCommentsAsync(postId);
           List<CommentModel> commentModels = new List<CommentModel>(); 
            foreach (var comment in comments)
            {
              var commentModel = _mapper.Map<CommentModel>(comment);
         
              var answerModels = _mapper.Map<List<AnswerModel>>(comment.Answers);
                commentModel.answerModels = answerModels;
                commentModels.Add(commentModel);
            }
            return Ok(commentModels);
        }
        [HttpGet("getCommentDetails/{commentId}")]
        public async Task<ActionResult> GetCommentAsync(Guid commentId)
        {
            return Ok();
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddCommentAsync(AddCommentModel commentModel)
        {
            var comment = _mapper.Map<Comment>(commentModel);
            await commentService.AddCommentAsync(comment);
            return Ok();
        }
        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<ActionResult> DeleteComment(Guid commentId)
        {
            await commentService.DeleteComment(commentId);
            return Ok();
        }
        [HttpPost("AnswerComment")]
        [Authorize]
        public async Task<ActionResult> AddAnswercomment(AddAnswerCommentmodel answerModel)
        {
            var answercomment = _mapper.Map<AnswerComment>(answerModel);
            await commentService.AddAnswercomment(answercomment);
            return Ok();
        }
    }
}
