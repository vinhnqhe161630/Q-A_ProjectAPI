
namespace PRN231_ProjectQA_MVC.Models.Comment
{
    public class AnswerModel
    {
       public Guid Id { get; set; }
        public string? Content { get; set; }
        public string? Img { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
         public Guid CommentId { get; set; }
        public string? Username { get; set; }

        public string Status { get; set; }
    }
}
