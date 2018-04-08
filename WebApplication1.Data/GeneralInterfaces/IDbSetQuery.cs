using ExecutionStrategyCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace WebApplication1.Data.GeneralInterfaces
{
    public interface IDbSetQuery<DbSetType, ReturnType> : IMapper<DbSet<DbSetType>, Task<InternalRunnerWrapper<ReturnType>>>
    where DbSetType : class
    { }
}
