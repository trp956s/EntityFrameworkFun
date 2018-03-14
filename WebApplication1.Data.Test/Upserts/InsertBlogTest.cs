using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;
using WebApplication1.Data.Upserts;
using FakeItEasy;

namespace WebApplication1.Data.Test.Upserts
{
    [TestClass]
    public class InsertBlogTest
    {
        [TestClass]
        public class Execute : InsertBlogTest
        {
            [TestMethod]
            public async Task InsertsBlogToDbSet()
            {
                var blogToInsert = new Blog();
                var insertBlog = new InsertBlog(blogToInsert);
                var upserter = A.Fake<IUpsertDbSet<Blog>>();

                await insertBlog.Execute(upserter);

                A.CallTo(() =>
                    upserter.AddAsync(A<Blog>.That.IsSameAs(blogToInsert))
                ).MustHaveHappenedOnceExactly();
            }
        }
    }
}
