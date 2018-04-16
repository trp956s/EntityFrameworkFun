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
    public class UpdateBlogTest
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

            bloggingContext.Blogs.Add(new Blog());
            bloggingContext.Blogs.Add(new Blog());
            bloggingContext.Blogs.Add(new Blog());
            bloggingContext.SaveChanges();
        }

        [TestClass]
        public class Run : UpdateBlogTest
        {
            [TestMethod]
            public async Task UpdatesTheRightRecord()
            {
                var oldBlogValues = bloggingContext.Blogs.ToArray().ElementAt(1);
                var newName = System.IO.Path.GetRandomFileName();
                var newBlogBValues = new Blog()
                {
                    Name = newName
                };

                var updater = new UpdateBlog(oldBlogValues, newBlogBValues);
                await runner.Run(updater, dbFake);

                var hasChanges = bloggingContext.ChangeTracker.HasChanges();

                Assert.AreEqual(3, bloggingContext.Blogs.Count());
                foreach (var blog in bloggingContext.Blogs.Where(x => x.Id != oldBlogValues.Id))
                {
                    Assert.AreNotEqual(blog.Name, newName);
                }
                Assert.AreEqual(bloggingContext.Blogs.Single(x=>x.Id == oldBlogValues.Id).Name, newName);
                Assert.IsFalse(hasChanges);
            }
        }
    }
}
