using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakeItEasy;
using static WebApplication1.Data.Queries.BlogPersistanceLayer;

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
                public void GetDataSetReturnsTheBlogDbSet()
                {
                    var context = new BloggingContext(new Microsoft.EntityFrameworkCore.DbContextOptions<BloggingContext>());
                    var dataset = _query.GetDataSet(context);

                    Assert.AreEqual(dataset, context.Blogs);
                }
            }
        }
    }
}
