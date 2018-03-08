using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Data.Helpers
{
    public interface IQuery<DataSetT, DbContextT, ReturnT> : IExecutable<DataSetT, ReturnT>
        where DataSetT : class
    {
        IEnumerable<DataSetT> GetDataEnumerable(DbContextT content);
    }

    public interface IExecutable<DataSetT, ReturnT>
        where DataSetT : class
    {
        Task<ReturnT> Execute(IAsyncEnumerable<DataSetT> queryable);
    }
}