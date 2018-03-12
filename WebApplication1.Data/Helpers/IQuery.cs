using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Data.Core;

namespace WebApplication1.Data.Helpers
{
    public interface IQuery<DataSetT, ReturnT> : IExecutable<IAsyncEnumerable<DataSetT>, ReturnT>
        where DataSetT : class
    {
    }
}