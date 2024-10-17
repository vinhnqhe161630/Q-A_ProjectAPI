using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.Entities
{
    public class Post_Tag
    {
        public Guid PostId { get; set; }
        [ForeignKey("PostId")] public virtual Post? Post { get; set; }
        public Guid TagId { get; set; }
        [ForeignKey("TagId")] public virtual Tag? Tag { get; set; }
    }
}
