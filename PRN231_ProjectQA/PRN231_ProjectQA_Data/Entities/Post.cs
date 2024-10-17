using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.Entities
{
    public class Post
    {
        public Post()
        {
            Title = string.Empty;
            Content1 = string.Empty;
            User = new User();
        }
        [Key] public Guid Id { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string Content1 { get; set; }
        public string? Image1 { get; set; }
        public string? Image2 { get; set; }
        public string? Content2 { get; set; }
        public int? ViewCount { get; set; }
        public int? Like { get; set; }
        [Required] public DateTime CreatedAt { get; set; }
        [Required] public Guid UserId { get; set; }

        [ForeignKey("UserId")] public virtual User User { get; set; }

        [Required] public PostStatus Status { get; set; }
        public virtual ICollection<Post_Tag>? Post_Tags { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Like>? Likes { get; set; }

        public enum PostStatus
        {
            Watting,
            Approved,
            Rejected,

        }
    }
}
