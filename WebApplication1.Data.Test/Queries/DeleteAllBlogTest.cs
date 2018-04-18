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
    public class DeleteAllTest
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

            bloggingContext.Blogs.Add(new Blog() { Id = 1233 });
            bloggingContext.Blogs.Add(new Blog() { Id = 1234 });
            bloggingContext.Blogs.Add(new Blog() { Id = 1235 });
            bloggingContext.SaveChanges();
        }

        [TestClass]
        public class Run : DeleteAllTest
        {
            [TestMethod]
            public async Task RemovesTheRightRecord()
            {
                var oldValues = bloggingContext.Blogs.ToArray();
                var deleteBlog = oldValues.First();
                var deleter = new DeleteAllById<Blog>(deleteBlog);

                await deleter.Run(bloggingContext);

                var newValues = bloggingContext.Blogs.ToArray();
                var hasChanges = bloggingContext.ChangeTracker.HasChanges();

                Assert.AreEqual(3, oldValues.Count());
                Assert.AreEqual(2, newValues.Count());
                Assert.IsFalse(hasChanges);
            }
        }
    }
}
