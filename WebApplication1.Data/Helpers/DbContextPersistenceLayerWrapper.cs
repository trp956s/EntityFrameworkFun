using System.Linq;

namespace WebApplication1.Data.Helpers
{
    public class DbContextPersistenceLayerWrapper
    {
        public ReturnT GetResults<DbContextT, DataSetT, ReturnT>(DbContextT dbContext, IQuery<DataSetT, DbContextT, ReturnT> query)
            where DataSetT : class
        {
            var ds = query.GetDataSet(dbContext);
            var queryable = ds.AsQueryable();
            return query.Execute(queryable);
        }
    }
}
