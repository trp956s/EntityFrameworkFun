using System;
using System.Collections.Generic;
using System.Text;
using WebApplication1.Data.Core;

namespace WebApplication1.Data.Helpers
{
    public interface IQueryMany<DataSetT> : IExecutable<IAsyncEnumerable<DataSetT>, IEnumerable<DataSetT>>
        where DataSetT : class
    {
    }
}
