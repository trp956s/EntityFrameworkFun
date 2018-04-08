using ExecutionStrategyCore;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebApplication1.Data.Models;
using WebApplication1.Data.Queries;

namespace WebApplication1.Data.Test.Queries
{
    [TestClass]
    public class BlogDbSetRunnerTest
    {
        BlogDbSetRunner blogDbSetRunner;
        IRunner<BloggingContext> contextWrapper;

        [TestInitialize]
        public void TestInit()
        {
            contextWrapper = A.Fake<IRunner<BloggingContext>>();
            blogDbSetRunner = new BlogDbSetRunner(contextWrapper);
        }

        [TestClass]
        public class Run : BlogDbSetRunnerTest
        {
            [TestMethod]
            public void ReturnsTheBlogsInTheWrappedContext()
            {
                var contextFake = Helpers.DbFake.CreateInMemoryDatabaseOptions<BloggingContext>();
                var bloggingContext = new BloggingContext(contextFake);
                var expectedDbSet = Helpers.DbFake.CreateAsyncEnumeratorDbSet<Blog>();
                A.CallTo(() => contextWrapper.Run()).
                    Returns(bloggingContext);
                bloggingContext.Blogs = expectedDbSet;

                var dbSet = blogDbSetRunner.Run();

                Assert.AreSame(dbSet, expectedDbSet);
            }
        }
    }
}
