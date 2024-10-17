using PRN231_ProjectQA_Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_ProjectQA_Data.IRepositories
{
    public interface ITagRepository
    {
        public Task<List<Tag>> GetTags();

    }
}
