namespace PRN231_ProjectQA_MVC.Models.User
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Img { get; set; }
        public string? DOB { get; set; }
        public string? Token { get; set; }
        public string RoleName { get; set; } = null!;
        public string Status { get; set; }
    }
}
