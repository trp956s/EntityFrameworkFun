using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;

namespace WebApplication1.Data.Queries.BlogPersistanceLayer
{        
    public class QueryAllBlogs : Queries.General.QueryAll<Blog, BloggingContext>
    {
        public override IEnumerable<Blog> GetDataEnumerable(BloggingContext content)
        {
            return content.Blogs;
        }
    }
}