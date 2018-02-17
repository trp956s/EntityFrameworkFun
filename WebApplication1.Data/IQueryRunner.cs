using WebApplication1.Data.Helpers;

namespace WebApplication1.Data
{
    public interface IQueryRunner
    {
        ReturnT Run<DataSetT, ReturnT>(IQuery<DataSetT, BloggingContext, ReturnT> query) where DataSetT : class;
    }
}