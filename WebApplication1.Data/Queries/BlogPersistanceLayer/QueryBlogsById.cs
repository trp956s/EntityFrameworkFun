using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries.BlogPersistanceLayer
{
    public class QueryBlogsById : IQuerySingle<Blog>
    {
        public int Id { get; private set; }

        public QueryBlogsById(int id)
        {
            Id = id;
        }

        public async Task<Blog> Execute(IAsyncEnumerable<Blog> queryable)
        {
            return await queryable.FirstOrDefault(b => b.Id == Id);
        }
    }
}
