﻿namespace PRN231_ProjectQA_WebAPI.Models.Auth
{
    public class AddUserModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? DOB { get; set; }
        public string RoleName { get; set; }
    }
}