namespace PRN231_ProjectQA_MVC.Models.Post
{
    public class PostDetailsModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Content1 { get; set; }
        public string? Image1 { get; set; }
        public string? Image2 { get; set; }
        public string? Content2 { get; set; }
        public int? TotalComment { get; set; }
        public string? UserImg { get; set; }
        public string? Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }



        public string Status { get; set; }
    }

}
