namespace PRN231_ProjectQA_MVC.Models.Post
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Content1 { get; set; }
        public string UserImg { get; set; }
        public string? CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public string? Username { get; set; }
        public int TotalComment { get; set; }
        
    }
}
