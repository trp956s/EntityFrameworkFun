using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Data.Core;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Upserts
{
    public class InsertBlog : IExecutable<IAsyncEnumerable<Blog>, Blog>
    {
        public Task<Blog> Execute(IAsyncEnumerable<Blog> queryable)
        {
            throw new System.NotImplementedException();
        }
    }
}
