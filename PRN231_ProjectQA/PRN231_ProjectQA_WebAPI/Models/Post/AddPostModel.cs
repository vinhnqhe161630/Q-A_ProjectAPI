namespace PRN231_ProjectQA_WebAPI.Models.Post
{
    public class AddPostModel
    {
      
        public string Title { get; set; } = string.Empty;
        public string Content1 { get; set; } = string.Empty;
        public string? Content2 { get; set; } = string.Empty;
        public string? Img1 { get; set; }
        public string? Img2 { get; set; }
        public Guid UserId { get; set; }
        
    }
}
