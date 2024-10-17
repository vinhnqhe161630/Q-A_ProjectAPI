using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.Entities
{
    public class AnswerComment
    {
        [Key] public Guid Id { get; set; }
        public string? Content { get; set; }
      
        public DateTime CreatedAt { get; set; }
        [Required] public Guid UserId { get; set; }
        [Required] public Guid CommentId { get; set; }
        [ForeignKey("CommentId")] public virtual Comment Comment { get; set; }
        [ForeignKey("UserId")] public virtual User User { get; set; }
        public AnswerCommentStatus Status { get; set; }
        public enum AnswerCommentStatus
        {
            InActive,
            Active,
            Locked,
        }
    }
}
