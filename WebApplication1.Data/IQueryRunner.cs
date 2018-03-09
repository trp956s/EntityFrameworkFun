using System.Threading.Tasks;
using WebApplication1.Data.Helpers;

namespace WebApplication1.Data
{
    public interface IQueryRunner
    {
        Task<ReturnT> Run<DataSetT, ReturnT>(IQuery<DataSetT, BloggingContext, ReturnT> query) where DataSetT : class;
    }

    public interface IBlogContext
    {
        Task<ReturnT> Run<DataSetT, ReturnT>(IExecutable<DataSetT, ReturnT> executable) where DataSetT : class;
    }
}