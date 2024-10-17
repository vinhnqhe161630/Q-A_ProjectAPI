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
    public class PostRepository : IPostRepository
    {
        private readonly DatabaseContext _context;  
        public PostRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<List<Post>> GetPostsByUser(Guid userId)
        {
            return await _context.Posts
                .Include(p => p.User)   
                .Include(p => p.Comments)
              .Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task AddNewPost(Post post)
        {
           

            try
            {
               
                var user = await _context.Users.FindAsync(post.UserId);
                post.User = user;
                _context.Posts.Add(post ?? throw new ArgumentNullException(nameof(post)));
                
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Log the detailed database exception
                throw new InvalidOperationException("An error occurred while saving the entity changes.", dbEx);
            }
            catch (Exception ex)
            {
                // Log the general exception
                throw new InvalidOperationException("An unexpected error occurred.", ex);
            }
        }

        public async Task DeletePost(Guid id)
        {
         var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Post>> GetAllPosts()
        {
            return await _context.Posts.
                Include(p => p.User)
                .Include(p => p.Comments)
               .ToListAsync();    

        }



        public async Task<Post> GetPostById(Guid id)
        {
            return await _context.Posts
          .Include(p => p.Comments)
            .Include(p => p.User)
          .FirstOrDefaultAsync(p => p.Id == id);
        }

      
       
    }
}
