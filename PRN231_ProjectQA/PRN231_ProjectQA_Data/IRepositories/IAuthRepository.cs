using PRN231_ProjectQA_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.IRepositories
{
    public interface IAuthRepository
    {
        public Task<User?> IsValidUser(string email, string password);
        public Task<User?> Login_Google(string email, string googleId);
        public Task<string> GetRoleUser(string email);
        public Task ResetPassword(User user);
        public Task<bool> IsUser(string email);
        public Task SetToken(string email, string token);
        public Task SignUp(User user);
        public Task<string> GetToken(string email);

        public Task<User?> GetUserByEmail(string email);
    }
}
