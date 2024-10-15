using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.Entities
{
    public class User
    {
        [Key] public Guid Id { get; set; }
        [Required] public string Password { get; set; }
        [Required, DataType(DataType.EmailAddress)] public string Email { get; set; }
        public string? Username { get; set; }
        [Required] public Guid RoleId { get; set; }
        public string? GoogleId { get; set; }
        public string? DOB { get; set; }
        public string? Token { get; set; }
        [Required] public UserStatus Status { get; set; }
        [ForeignKey("RoleId")] public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Like>? Likes { get; set; }

        public enum UserStatus
        {
            Inactive,
            Active
        }
    }
}
