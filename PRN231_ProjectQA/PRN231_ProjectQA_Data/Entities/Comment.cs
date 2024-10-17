using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PRN231_ProjectQA_Data.Entities
{
    public class Comment
    {
        public Comment()
        {
            Content = string.Empty;
            Post = new Post();
            User = new User();
        }

        [Key] public Guid Id { get; set; }
        [Required] public string Content { get; set; }
        [Required] public DateTime CreatedAt { get; set; }
     
        [Required] public Guid PostId { get; set; }
        [ForeignKey("PostId")]
        [InverseProperty("Comments")]
        public virtual Post? Post { get; set; }
        [Required] public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Comments")]
        public virtual User? User { get; set; }
        public virtual ICollection<AnswerComment>? Answers { get; set; }
        [Required] public CommentStatus Status { get; set; }
        public enum CommentStatus
        {
            InActive,
            Active,
            Locked,
        }
    }
}