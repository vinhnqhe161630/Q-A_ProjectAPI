﻿namespace PRN231_ProjectQA_WebAPI.Models.User
{
    public class AddUserModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
      
        public string RoleName { get; set; }
    }
}
