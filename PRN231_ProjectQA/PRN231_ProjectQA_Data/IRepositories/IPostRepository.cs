using PRN231_ProjectQA_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.IRepositories
{
    public interface IPostRepository
    {
        public Task<List<Post>> GetAllPosts();
        public Task<Post> GetPostById(Guid id);
        public Task<List<Post>> GetPostsByUser(Guid userId);
        public Task AddNewPost(Post post);
      
        public Task DeletePost(Guid id);
        public Task UpdatePost(Post post);  
     

    }
}
