using PRN231_ProjectQA_Data.Entities;
using PRN231_ProjectQA_Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Service.Services
{
    public class CommentService
    {
        private readonly ICommentRepository commentRepository;
        public CommentService(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }
        public async Task<List<Comment>> GetCommentsAsync(Guid postId)
        {
            return await commentRepository.GetCommentsAsync(postId);
        }
        public async Task<Comment> GetCommentAsync(Guid commentId)
        {
            return await commentRepository.GetCommentAsync(commentId);
        }
        public async Task AddCommentAsync(Comment comment)

        {
            await commentRepository.AddCommentAsync(comment);
        }
        public async Task DeleteComment(Guid commentId)
        {
            await commentRepository.DeleteComment(commentId);
        }
        public async Task AddAnswercomment(AnswerComment answerComment)
        {
            await commentRepository.AddAnswercomment(answerComment);
        }

    }
}
