using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries.BlogPersistanceLayer
{
    public class QueryBlogsById : IQuery<Blog, BloggingContext, Blog>
    {
        public int Id { get; private set; }

        public QueryBlogsById(int id)
        {
            Id = id;
        }
        public IEnumerable<Blog> GetDataEnumerable(BloggingContext content)
        {
            return content.Blogs;
        }

        public async Task<Blog> Execute(IAsyncEnumerable<Blog> queryable)
        {
            return await queryable.FirstOrDefault(b => b.Id == Id);
        }
    }
}
