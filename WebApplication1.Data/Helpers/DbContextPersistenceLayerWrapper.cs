using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data.Helpers
{
    public class DbContextPersistenceLayerWrapper
    {
        public async Task<ReturnT> GetResults<DbContextT, DataSetT, ReturnT>(DbContextT dbContext, IQuery<DataSetT, DbContextT, ReturnT> query)
            where DataSetT : class
        {
            var ds = query.GetDataEnumerable(dbContext);
            return await query.Execute(ds.ToAsyncEnumerable());
        }
    }
}
