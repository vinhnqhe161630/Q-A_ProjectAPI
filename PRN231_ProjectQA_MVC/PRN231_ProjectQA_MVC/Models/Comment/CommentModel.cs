namespace PRN231_ProjectQA_MVC.Models.Comment
{
    public class CommentModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Img { get; set; }
        public Guid PostId { get; set; }
        public string? Username { get; set; }
        public Guid UserId { get; set; }

        public string Status { get; set; }
        public List<AnswerModel>? answerModels { get; set; }

    }
}
