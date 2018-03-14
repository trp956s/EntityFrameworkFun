using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Data.Core;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Upserts
{
    public class InsertBlog : IExecutable<IAsyncEnumerable<Blog>, Blog>
    {
        private Blog blogToInsert;

        public InsertBlog(Blog blogToInsert)
        {
            this.blogToInsert = blogToInsert;
        }

        public Task<Blog> Execute(IAsyncEnumerable<Blog> queryable)
        {
            throw new System.NotImplementedException();
        }

        public async Task Execute(IUpsertDbSet<Blog> blogInserter)
        {
            await blogInserter.AddAsync(blogToInsert);
            await blogInserter.Save();
        }
    }
}
