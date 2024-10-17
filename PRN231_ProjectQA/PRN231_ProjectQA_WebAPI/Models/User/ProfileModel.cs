﻿using PRN231_ProjectQA_Data.Entities;

namespace PRN231_ProjectQA_WebAPI.Models.User
{
    public class ProfileModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Img { get; set; }
        public int QAsked { get; set; }
        public int QAnswered { get; set; }
      

    }
}
