using System.Threading.Tasks;
using WebApplication1.Data.Helpers;
using WebApplication1.Data.Models;
using System.Collections.Generic;

namespace WebApplication1.Data
{
    public interface IQueryRunner
    {
        Task<ReturnT> Run<DataSetT, ReturnT>(IQuery<DataSetT, BloggingContext, ReturnT> query) where DataSetT : class;
    }

    public interface IBlogContext: IDbSetLookup<Blog>
    {
    }

    public interface IDbSetLookup<T> where T: class
    {
        Task<T> Run(IExecutable<IAsyncEnumerable<T>, T> executable);
    }
}