using System.Threading.Tasks;
using WebApplication1.Data.Core;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Upserts
{
    public class InsertBlog : IExecutable<IUpsertDbSet<Blog>, int>
    {
        public Blog BlogToInsert { get; }

        public InsertBlog(Blog blogToInsert)
        {
            this.BlogToInsert = blogToInsert;
        }

        public async Task<int> Execute(IUpsertDbSet<Blog> blogInserter)
        {
            await blogInserter.AddAsync(BlogToInsert);
            return await blogInserter.Save();
        }
    }
}
