using ExecutionStrategyCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplication1.Data.Queries
{
    public struct BloggingContextRunner : IRunner<BloggingContext>
    {
        private readonly BloggingContext context;

        public BloggingContextRunner(BloggingContext context) {
            this.context = context;
        }

        public BloggingContext Run()
        {
            return context;
        }
    }
}
