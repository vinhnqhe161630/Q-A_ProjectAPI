using PRN231_ProjectQA_Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRN231_ProjectQA_Data.Entities;   

namespace PRN231_ProjectQA_Service.Services
{
    public class PostService
    {
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
       public PostService(IPostRepository postRepository, IUserRepository userRepository )
        {
            this.postRepository = postRepository;
            this.userRepository = userRepository;
        }   

        public async Task<Post> GetPostById(Guid id)
        {
            return await postRepository.GetPostById(id);
        }   
        public async Task AddNewPost(Post post)
        {
            post.CreatedAt = DateTime.Now;
            post.Status = Post.PostStatus.Watting;
            post.Id = Guid.NewGuid();   
            
            await postRepository.AddNewPost(post);
        }

        public async Task DeletePost(Guid id)
        {
            await postRepository.DeletePost(id);
        }

        public async Task<List<Post>> GetAllPosts()
        {
            return await postRepository.GetAllPosts();
        }
        public async Task<List<Post>> GetPostsByUser(Guid userId)
        {
            var user = await userRepository.GetUserById(userId);    
            if(user == null)
            {
                throw new Exception("User not found.");
            }   

            return await postRepository.GetPostsByUser(userId);
        }
        public async Task UpdatePost(Post post)
        {
            await postRepository.UpdatePost(post);
        }

    }
}
