using static PRN231_ProjectQA_Data.Entities.Comment;

namespace PRN231_ProjectQA_WebAPI.Models.Comment
{
    public class AddCommentModel
    {
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }

        
    }
}
