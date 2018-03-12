using System.Collections.Generic;
using WebApplication1.Data.Core;

namespace WebApplication1.Data.Helpers
{
    public interface IQuerySingle<DataSetT> : IExecutable<IAsyncEnumerable<DataSetT>, DataSetT>
        where DataSetT : class
    {
    }
}