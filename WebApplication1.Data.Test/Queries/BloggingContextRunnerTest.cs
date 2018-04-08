using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using WebApplication1.Data.Queries;

namespace WebApplication1.Data.Test.Queries
{
    [TestClass]
    public class BloggingContextRunnerTest
    {
        BloggingContextRunner bloggingContextRunner;
        BloggingContext bloggingContext;

        [TestInitialize]
        public void TestInit()
        {
            var contextFake = Helpers.DbFake.CreateInMemoryDatabaseOptions<BloggingContext>();
            bloggingContext = new BloggingContext(contextFake);
            bloggingContextRunner = new BloggingContextRunner(bloggingContext);
        }

        [TestClass]
        public class Run : BloggingContextRunnerTest
        {
            [TestMethod]
            public void ReturnsTheContext()
            {
                var actualContext = bloggingContextRunner.Run();

                Assert.AreSame(bloggingContext, actualContext);
            }
        }
    }
}
