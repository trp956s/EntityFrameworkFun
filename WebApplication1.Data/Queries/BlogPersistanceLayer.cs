using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            private readonly int _id;

            public QueryById(int id)
            {
                _id = id;
            }
            public DbSet<Blog> GetDataSet(BloggingContext content)
            {
                return content.Blogs;
            }

            public Blog Execute(IQueryable<Blog> queryable)
            {
                throw new NotImplementedException();
            }
        }
    }
}
