using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Data.Queries.BlogPersistanceLayer;
using System.Collections.Generic;
using WebApplication1.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data.Test.Queries.BlogPersistanceLayer
{
    [TestClass]
    public class QueryAllTest
    {
        [TestClass]
        public class Execute: QueryAllTest
        {
            [TestMethod]
            public async Task ReturnsAllBlogs()
            {
                var blogs = new List<Blog> {new Blog(), new Blog()};
                var queryAll = new QueryAllBlogs();

                var result = await queryAll.Execute(blogs.ToAsyncEnumerable());

                CollectionAssert.AreEqual(blogs, result.ToList());
            }
        }

        [TestClass]
        public class GetDataSet : QueryAllTest
        {
            [TestMethod]
            public void ReturnsBlogs()
            {
                var queryAll = new QueryAllBlogs();
                var context = new BloggingContext(new Microsoft.EntityFrameworkCore.DbContextOptions<BloggingContext>());

                var dataSet = queryAll.GetDataEnumerable(context);

                Assert.AreEqual(context.Blogs, dataSet);
            }
        }
    }
}