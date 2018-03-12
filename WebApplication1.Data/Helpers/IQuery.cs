using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Data.Helpers
{
    public interface IQuery<DataSetT, DbContextT, ReturnT> : IExecutable<IAsyncEnumerable<DataSetT>, ReturnT>
        where DataSetT : class
    {
        IEnumerable<DataSetT> GetDataEnumerable(DbContextT content);
    }

    public interface IExecutable<InputTypeT, ReturnT>
        where InputTypeT : class
    {
        Task<ReturnT> Execute(InputTypeT queryable);
    }
}