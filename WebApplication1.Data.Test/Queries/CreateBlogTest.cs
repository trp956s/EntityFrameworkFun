using ExecutionStrategyCore;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data.Models;
using WebApplication1.Data.Queries;
using WebApplication1.Data.Test.Helpers;

namespace WebApplication1.Data.Test.Queries
{
    [TestClass]
    public class CreateBlogTest
    {
        ExecutionStrategyRunner runner;
        IRunner<BloggingContext> dbFake;
        BloggingContext bloggingContext;

        [TestInitialize]
        public void TestInitialize()
        {
            runner = new ExecutionStrategyRunner();
            dbFake = A.Fake<IRunner<BloggingContext>>();

            var dbOptions = DbFake.CreateInMemoryDatabaseOptions<BloggingContext>();
            bloggingContext = new BloggingContext(dbOptions);
            A.CallTo(() => dbFake.Run()).Returns(bloggingContext);
        }

        [TestClass]
        public class Run : CreateBlogTest
        {
            [TestMethod]
            public async Task AddsBlogAndSaves()
            {
                var expectedBlog = new Blog() { Name = System.IO.Path.GetRandomFileName() };
                var createBlog = new CreateBlog(expectedBlog);

                await runner.Run(createBlog, dbFake);

                Assert.AreEqual(1, bloggingContext.Blogs.Count());
                Assert.AreSame(expectedBlog, bloggingContext.Blogs.First());
            }
        }
    }
}