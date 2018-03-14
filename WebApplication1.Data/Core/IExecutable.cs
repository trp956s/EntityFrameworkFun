using System.Threading.Tasks;

namespace WebApplication1.Data.Core
{

    public interface IExecutable<InputTypeT, ReturnT>
        where InputTypeT : class
    {
        Task<ReturnT> Execute(InputTypeT queryable);
    }
}
