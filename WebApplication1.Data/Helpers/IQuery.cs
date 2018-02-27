using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Data.Helpers
{
    public interface IQuery<DataSetT, DbContextT, ReturnT>
        where DataSetT : class
    {
        IEnumerable<DataSetT> GetDataEnumerable(DbContextT content);
        Task<ReturnT> Execute(IAsyncEnumerable<DataSetT> queryable);
    }
}