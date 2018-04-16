using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Data.Queries
{
    public struct DeleteAllById<T> : IMapper<BloggingContext, Task<InternalRunnerWrapper<int>>>
    {
        private int deleteId;

        public DeleteAllById(int deleteId)
        {
            this.deleteId = deleteId;
        }

        public Task<InternalRunnerWrapper<int>> Run(BloggingContext arg)
        {
            throw new NotImplementedException();
        }
    }
}
