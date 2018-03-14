using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Data.Queries.BlogPersistanceLayer;
using System.Collections.Generic;
using WebApplication1.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data.Test.Queries.BlogPersistanceLayer
{
    [TestClass]
    public class QueryByIdTest
    {
        public int _id;
        public QueryBlogsById _query;

        [TestInitialize]
        public void Initialize()
        {
            _id = 9;
            _query = new QueryBlogsById(_id);
        }

        [TestClass]
        public class Execute : QueryByIdTest
        {
            [TestMethod]
            public async Task ReturnsNullWhenEmptyDataSet()
            {
                var blogs = new List<Blog>().ToAsyncEnumerable();

                var result = await _query.Execute(blogs);

                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ReturnsNullWhenNoMatchFound()
            {
                var blogs = new List<Blog> { new Blog() { Id = 0 } }.ToAsyncEnumerable();

                var result = await _query.Execute(blogs);

                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ReturnsBlogWhenMatchFound()
            {
                var blog = new Blog() { Id = _id };
                var blogs = new List<Blog> { blog }.ToAsyncEnumerable();

                var result = await _query.Execute(blogs);

                Assert.AreEqual(blog, result);
            }

            [TestMethod]
            public async Task ReturnsFirstMatchingBlog()
            {
                var blog = new Blog() { Id = _id };
                var blogs = new List<Blog> { blog, new Blog() { Id = _id } }.ToAsyncEnumerable();

                var result = await _query.Execute(blogs);

                Assert.AreEqual(blog, result);
            }
        }
    }
}
