using Microsoft.EntityFrameworkCore;
using PRN231_ProjectQA_Data.DataContext;
using PRN231_ProjectQA_Data.Entities;
using PRN231_ProjectQA_Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PRN231_ProjectQA_Data.Entities.User;

namespace PRN231_ProjectQA_Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        //Get list user
        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users!
                .Include(u => u.Role).ToListAsync();
        }
        public async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users!
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
        //Update user status
        public async Task UpdateUserStatus(Guid id, UserStatus status)
        {
            var user = await _context.Users!.FindAsync(id);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await _context.Users!.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            _context.Users!.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task AddNewUser(User user)
        {

            var existingUser = _context.Users!.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already exists.");
            }

            var newUser = new User
            {
                Username = user.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Email = user.Email,
                DOB = user.DOB,
              
                RoleId = user.RoleId,
                Status = UserStatus.Active,
            };


            await _context.Users!.AddAsync(newUser);
            await _context.SaveChangesAsync();

        }

        public async Task UpdateUser(Guid id, User user)
        {
            var updateUser = await _context.Users!.FindAsync(user.Id) ?? throw new Exception("User not found");
            updateUser.Email = user.Email;
            updateUser.DOB = user.DOB;
            updateUser.Username = user.Username;
            updateUser.RoleId = user.RoleId;

            await _context.SaveChangesAsync();

        }

        public async Task<Guid?> GetRoleIdbyName(string roleName)
        {
            var role = await _context.Roles
         .FirstOrDefaultAsync(r => r.Name.Equals(roleName));



            return role?.Id;
        }


    }
}


