using static PRN231_ProjectQA_Data.Entities.Comment;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PRN231_ProjectQA_WebAPI.Models.Comment
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
