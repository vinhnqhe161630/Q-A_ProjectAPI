﻿namespace PRN231_ProjectQA_WebAPI.Models.Auth
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? DOB { get; set; }
        public string? Token { get; set; }
        public string RoleName { get; set; } = null!;
        public string Status { get; set; }
    }
}
