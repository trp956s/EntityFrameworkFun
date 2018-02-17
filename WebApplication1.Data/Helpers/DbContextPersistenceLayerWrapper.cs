using System.Linq;

namespace WebApplication1.Data.Helpers
{
    public class DbContextPersistenceLayerWrapper
    {
        public ReturnT GetResults<DbContextT, QueryParametersT, DataSetT, ReturnT>(DbContextT dbContext, IQuery<QueryParametersT, DataSetT, DbContextT, ReturnT> query, QueryParametersT parameters)
            where DataSetT : class
        {
            var ds = query.GetDataSet(dbContext);
            var queryable = ds.AsQueryable();
            return query.Execute(queryable, parameters);
        }
    }
}
