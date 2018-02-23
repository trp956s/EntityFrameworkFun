using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Extensions.Internal;

namespace WebApplication1.Data.Helpers
{
    public class DbContextPersistenceLayerWrapper
    {
        public async Task<ReturnT> GetResults<DbContextT, DataSetT, ReturnT>(DbContextT dbContext, IQuery<DataSetT, DbContextT, ReturnT> query)
            where DataSetT : class
        {
            var ds = query.GetDataSet(dbContext);
            return await query.Execute(ds.AsyncEnumerable);
        }
    }
}
