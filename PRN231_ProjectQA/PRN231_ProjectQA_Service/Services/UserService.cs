using PRN231_ProjectQA_Data.Entities;
using PRN231_ProjectQA_Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PRN231_ProjectQA_Data.Entities.User;

namespace PRN231_ProjectQA_Service.Services
{
    public class UserService
    {
        private readonly IUserRepository _userrepository;
        public UserService(IUserRepository userRepository)
        {

            _userrepository = userRepository;


        }
        //Get all user
        public async Task<List<User>> GetAllUsers()
        {
            var result = await _userrepository.GetAllUsers();
            return result;
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _userrepository.GetUserById(id);
            return user;
        }
        public async Task UpdateUserStatus(Guid id, UserStatus status)
        {
            await _userrepository.UpdateUserStatus(id, status);
        }
        //Delete user
        public async Task DeleteUser(Guid id)
        {
            await _userrepository.DeleteUser(id);
        }
        public async Task UpdateUser(Guid id, User user)
        {
            await _userrepository.UpdateUser(id, user);
        }

        public async Task AddNewUser(User user)
        {
            await _userrepository.AddNewUser(user);

        }
        public async Task<Guid?> GetRoleIdByName(string roleName)
        {

            return await _userrepository.GetRoleIdbyName(roleName);



        }

    }
}
