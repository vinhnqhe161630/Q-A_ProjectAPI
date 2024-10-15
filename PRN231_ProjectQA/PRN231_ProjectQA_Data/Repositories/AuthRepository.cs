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
    public class AuthRepository : IAuthRepository
    {
        private readonly DatabaseContext _context;
        public AuthRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<User?> IsValidUser(string email, string password)
        {

            // Truy vấn người dùng theo email
            var user = await _context.Users!.FirstOrDefaultAsync(u => u.Email == email);

            // Nếu không tìm thấy người dùng, trả về null
            if (user == null)
            {
                return null;
            }

            // Kiểm tra mật khẩu bằng cách sử dụng BCrypt
            if (BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user; // Mật khẩu chính xác, trả về người dùng
            }



            return null; // Mật khẩu không chính xác hoặc lỗi xảy ra
        }
        public async Task<string> GetRoleUser(string email)
        {
            if (_context.Users == null)
            {
                return string.Empty;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return string.Empty;
            }

            var role = await _context.Roles!.FirstOrDefaultAsync(r => r.Id == user.RoleId);
            return role != null ? role.Name : string.Empty;
        }
        public async Task ResetPassword(User user)
        {
            var userToUpdate = await _context.Users!.SingleOrDefaultAsync(u => u.Email == user.Email) ??
                               throw new Exception("User not found");
            userToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            userToUpdate.Token = null;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUser(string email)
        {
            var user = await _context.Users!.FirstOrDefaultAsync(u => u.Email == email && u.Status == User.UserStatus.Active);
            return user != null;
        }

        public async Task SetToken(string email, string token)
        {
            var user = await _context.Users!.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.Token = token;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GetToken(string email)
        {
            var user = await _context.Users!.FirstOrDefaultAsync(u => u.Email == email) ??
                       throw new Exception("User not found");
            return user.Token!;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users!.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> Login_Google(string email, string googleId)
        {
            var user = await _context.Users!.FirstOrDefaultAsync(u => u.Email == email && u.GoogleId == googleId);
            if (user != null)
            {
                return user;
            }
            return null;
        }
        public async Task SignUp(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
