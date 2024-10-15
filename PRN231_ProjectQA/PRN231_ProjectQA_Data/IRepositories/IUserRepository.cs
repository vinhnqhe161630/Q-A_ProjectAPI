using PRN231_ProjectQA_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PRN231_ProjectQA_Data.Entities.User;

namespace PRN231_ProjectQA_Data.IRepositories
{
    public interface IUserRepository
    {
        public Task AddNewUser(User user);
        public Task UpdateUser(Guid id, User user);
        public Task<List<User>> GetAllUsers();
        public Task<User> GetUserById(Guid id);
        public Task DeleteUser(Guid id);
        public Task UpdateUserStatus(Guid id, UserStatus status);

        public Task<Guid?> GetRoleIdbyName(string roleName);
    }
}
