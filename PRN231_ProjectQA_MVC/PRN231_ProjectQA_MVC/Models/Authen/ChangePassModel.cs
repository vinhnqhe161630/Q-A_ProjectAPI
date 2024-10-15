namespace PRN231_ProjectQA_MVC.Models.Authen
{
    public class ChangePassModel
    {
        public string Token { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
