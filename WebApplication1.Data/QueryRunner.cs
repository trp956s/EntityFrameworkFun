using WebApplication1.Data.Helpers;

namespace WebApplication1.Data
{
    public class QueryRunner
    {
        
        public QueryRunner(BloggingContext bloggingContent)
        {
            BloggingContent = bloggingContent;
            Wrapper = new DbContextPersistenceLayerWrapper();
        }

        public BloggingContext BloggingContent { get; private set; }
        public DbContextPersistenceLayerWrapper Wrapper { get; private set; }

        public ReturnT Run<QueryParametersT, DataSetT, ReturnT>(IQuery<QueryParametersT, DataSetT, BloggingContext, ReturnT> query, QueryParametersT parameters)
            where DataSetT : class
        {
            return Wrapper.GetResults(this.BloggingContent, query, parameters);
        }
    }
}
