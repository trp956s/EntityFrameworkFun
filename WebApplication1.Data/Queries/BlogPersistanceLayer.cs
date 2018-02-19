using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public class QueryById : IQuery<Blog, BloggingContext, Blog>
        {
            public int Id { get; private set; }

            public QueryById(int id)
            {
                Id = id;
            }
            public DbSet<Blog> GetDataSet(BloggingContext content)
            {
                return content.Blogs;
            }

            public async Task<Blog> Execute(IQueryable<Blog> queryable)
            {
                return queryable.FirstOrDefault(b => b.Id == Id);
            }
        }
    }
}
