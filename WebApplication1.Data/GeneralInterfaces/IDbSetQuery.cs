using ExecutionStrategyCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data.GeneralInterfaces
{
    public interface IDbSetQuery<DbSetType, ReturnType> : IMapper<IQueryable<DbSetType>, Task<InternalValueCache<ReturnType>>>
    where DbSetType : class
    { }
}
