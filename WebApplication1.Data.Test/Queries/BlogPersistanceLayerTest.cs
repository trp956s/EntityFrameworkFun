using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeItEasy;
using static WebApplication1.Data.Queries.BlogPersistanceLayer;
using System.Collections.Generic;
using WebApplication1.Data.Models;
using System.Linq;

namespace WebApplication1.Data.Test.Queries
{
    [TestClass]
    public class BlogPersistanceLayerTest
    {
        [TestClass]
        public class QueryByIdTest : BlogPersistanceLayerTest
        {
            public int _id;
            public QueryById _query;

            [TestInitialize]
            public void Initialize()
            {
                _id = 9;
                _query = new QueryById(_id);
            }

            [TestClass]
            public class GetDataSet : QueryByIdTest
            {
                [TestMethod]
                public void ReturnsTheBlogDbSet()
                {
                    var context = new BloggingContext(new Microsoft.EntityFrameworkCore.DbContextOptions<BloggingContext>());
                    var dataset = _query.GetDataSet(context);

                    Assert.AreEqual(dataset, context.Blogs);
                }
            }

            [TestClass]
            public class Execute : QueryByIdTest
            {
                [TestMethod]
                public void ReturnsNullWhenEmptyDataSet()
                {
                    var blogs = new List<Blog>().AsQueryable();

                    var result = _query.Execute(blogs);

                    Assert.IsNull(result);
                }

                [TestMethod]
                public void ReturnsNullWhenNoMatchFound()
                {
                    var blogs = new List<Blog> { new Blog() { Id = 0 } }.AsQueryable();

                    var result = _query.Execute(blogs);

                    Assert.IsNull(result);
                }

                [TestMethod]
                public void ReturnsBlogWhenMatchFound()
                {
                    var blog = new Blog() { Id = _id };
                    var blogs = new List<Blog> { blog }.AsQueryable();

                    var result = _query.Execute(blogs);

                    Assert.AreEqual(blog, result);
                }

                [TestMethod]
                public void ReturnsFirstMatchingBlog()
                {
                    var blog = new Blog() { Id = _id };
                    var blogs = new List<Blog> { blog, new Blog() { Id = _id } }.AsQueryable();

                    var result = _query.Execute(blogs);

                    Assert.AreEqual(blog, result);
                }

            }
        }
    }
}
