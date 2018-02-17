using WebApplication1.Data.Helpers;

namespace WebApplication1.Data
{
    public class QueryRunner
    {
        private readonly BloggingContext _bloggingContent;
        private readonly DbContextPersistenceLayerWrapper _wrapper;

        public QueryRunner(BloggingContext bloggingContent)
        {
            _bloggingContent = bloggingContent;
            _wrapper = new DbContextPersistenceLayerWrapper();
        }

        public ReturnT Run<DataSetT, ReturnT>(IQuery<DataSetT, BloggingContext, ReturnT> query)
            where DataSetT : class
        {
            return _wrapper.GetResults(this._bloggingContent, query);
        }
    }
}
