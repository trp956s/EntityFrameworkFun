using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;
using WebApplication1.Data.Upserts;
using FakeItEasy;
using System;

namespace WebApplication1.Data.Test.Upserts
{
    [TestClass]
    public class InsertBlogTest
    {
        [TestClass]
        public class Execute : InsertBlogTest
        {
            [TestMethod]
            public async Task TracksAndSavesBlogToDbSet()
            {
                var blogToInsert = new Blog();
                var insertBlog = new InsertBlog(blogToInsert);
                var upserter = A.Fake<IUpsertDbSet<Blog>>();

                await insertBlog.Execute(upserter);

                A.CallTo(() =>
                    upserter.AddAsync(A<Blog>.That.IsSameAs(blogToInsert))
                ).MustHaveHappenedOnceExactly();

                A.CallTo(() =>
                    upserter.Save()
                ).MustHaveHappenedOnceExactly();
            }

            [TestMethod]
            public async Task DoesNotSaveBlogIfAddThrows()
            {
                var expectedException = new Exception();
                var insertBlog = new InsertBlog(null);
                var upserter = A.Fake<IUpsertDbSet<Blog>>();

                A.CallTo(() =>
                    upserter.AddAsync(A<Blog>.Ignored)
                ).Throws(expectedException);

                await Assert.ThrowsExceptionAsync<Exception>(async () =>
                    await insertBlog.Execute(upserter)
                );

                A.CallTo(() =>
                    upserter.Save()
                ).MustNotHaveHappened();
            }
        }
    }
}
