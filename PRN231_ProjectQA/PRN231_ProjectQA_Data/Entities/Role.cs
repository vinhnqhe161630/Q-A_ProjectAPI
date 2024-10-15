using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.Entities
{
    public class Role
    {
        [Key] public Guid Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public RoleStatus Status { get; set; }
        public virtual ICollection<User>? Users { get; set; }
        public enum RoleStatus
        {
            Inactive,
            Active
        }
    
}
}
