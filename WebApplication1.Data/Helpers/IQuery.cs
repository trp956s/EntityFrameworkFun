using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data.Helpers
{
    public interface IQuery<DataSetT, DbContextT, ReturnT>
        where DataSetT : class
    {
        DbSet<DataSetT> GetDataSet(DbContextT content);
        Task<ReturnT> Execute(IQueryable<DataSetT> queryable);
    }
}