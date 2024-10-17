using PRN231_ProjectQA_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.IRepositories
{
   public interface ICommentRepository
    {
        public Task<List<Comment>> GetCommentsAsync(Guid postId);
        public Task<Comment> GetCommentAsync(Guid commentId);
        public Task AddCommentAsync(Comment comment);
        public Task DeleteComment(Guid commentId);  
        public Task AddAnswercomment(AnswerComment answerComment);
        
       
    }
}
