using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.Entities
{
    public class Like
    {
        public Guid PostId { get; set; }
        [ForeignKey("PostId")] public virtual Post Post { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")] public virtual User User { get; set; }
    }
}
