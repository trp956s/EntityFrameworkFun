using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace WebApplication1.Data.Helpers
{
    public interface IQuery<DataSetT, DbContextT, ReturnT>
        where DataSetT : class
    {
        IAsyncEnumerableAccessor<DataSetT> GetDataSet(DbContextT content);
        Task<ReturnT> Execute(IAsyncEnumerable<DataSetT> queryable);
    }
}