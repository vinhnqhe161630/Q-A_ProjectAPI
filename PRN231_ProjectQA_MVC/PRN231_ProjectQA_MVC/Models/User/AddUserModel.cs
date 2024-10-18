namespace PRN231_ProjectQA_MVC.Models.User
{
    public class AddUserModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        
        public string RoleName { get; set; }
    }
}
