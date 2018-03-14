using System.Collections.Generic;
using WebApplication1.Data.Core;

namespace WebApplication1.Data.Helpers
{
    public interface IQueryMany<DataSetT> : IExecutable<IAsyncEnumerable<DataSetT>, IEnumerable<DataSetT>>
        where DataSetT : class
    {
    }
}
