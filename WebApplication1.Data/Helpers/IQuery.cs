using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data.Helpers
{
    public interface IQuery<DataSetT, DbContextT, ReturnT>
        where DataSetT : class
    {
        DbSet<DataSetT> GetDataSet(DbContextT content);
        ReturnT Execute(IQueryable<DataSetT> queryable);
    }
}