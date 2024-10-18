using Microsoft.EntityFrameworkCore;
using PRN231_ProjectQA_Data.DataContext;
using PRN231_ProjectQA_Data.Entities;
using PRN231_ProjectQA_Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DatabaseContext context;
        public CommentRepository(DatabaseContext context)
        {
            this.context = context; 
        }

        public async Task AddAnswercomment(AnswerComment answerComment)
        {
            if (answerComment == null)
            {
                throw new ArgumentNullException(nameof(answerComment));
            }

          
            var comment = await context.Comments.FindAsync(answerComment.CommentId);
            if (comment == null)
            {
                throw new InvalidOperationException($"Comment with ID {answerComment.CommentId} not found.");
            }
            var user = await context.Users.FindAsync(answerComment.UserId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {answerComment.UserId} not found.");
            }
            answerComment.User = user;
            answerComment.Comment = comment;
            context.AnswerComments.Add(answerComment);
            await context.SaveChangesAsync();
        }

        public async Task AddCommentAsync(Comment comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            if (comment.UserId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty.", nameof(comment.UserId));
            }

            if (comment.PostId == Guid.Empty)
            {
                throw new ArgumentException("PostId cannot be empty.", nameof(comment.PostId));
            }

            var user = await context.Users.FindAsync(comment.UserId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {comment.UserId} not found.");
            }

            var post = await context.Posts.FindAsync(comment.PostId);
            if (post == null)
            {
                throw new InvalidOperationException($"Post with ID {comment.PostId} not found.");
            }
            comment.User = user;
            comment.Post = post;

            context.Comments.Add(comment);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Log the detailed database exception
                throw new InvalidOperationException("An error occurred while saving the entity changes.", dbEx);
            }
        }


        public Task DeleteComment(Guid commentId)
        {
            context.AnswerComments.RemoveRange(context.AnswerComments.Where(a => a.CommentId == commentId));
         context.Comments.Remove(context.Comments.Find(commentId));
            return context.SaveChangesAsync();
        }

        public Task<Comment> GetCommentAsync(Guid commentId)
        {
           return context.Comments.Include(c => c.Answers).Include(c => c.User).FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<List<Comment>> GetCommentsAsync(Guid postId)
        {
            return await context.Comments
                .Include(c => c.Answers)
                .Include(c => c.User)
                .Where(c => c.PostId == postId).OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        }
    }
}
