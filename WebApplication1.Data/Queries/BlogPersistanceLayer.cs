using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries
{
    public class BlogPersistanceLayer
    {
        public QueryById GetById(int id)
        {
            return new QueryById(id);
        }

        public QueryAll GetAll()
        {
            return new QueryAll();
        }

        public class QueryById : IQuery<Blog, BloggingContext, Blog>
        {
            public int Id { get; private set; }

            public QueryById(int id)
            {
                Id = id;
            }
            public IAsyncEnumerableAccessor<Blog> GetDataSet(BloggingContext content)
            {
                return content.Blogs;
            }

            public async Task<Blog> Execute(IAsyncEnumerable<Blog> queryable)
            {
                return await queryable.FirstOrDefault(b => b.Id == Id);
            }
        }

        public class QueryAll : Queries.General.QueryAll<Blog, BloggingContext>
        {

        }
    }
}
