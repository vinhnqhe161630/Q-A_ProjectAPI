using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.Entities
{
    public class Tag
    {
        [Key] public Guid Id { get; set; }
        [Required] public string Name { get; set; }

        public virtual ICollection<Post_Tag>? Post_Tags { get; set; }
    }
}
