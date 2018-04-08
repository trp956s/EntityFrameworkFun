using ExecutionStrategyCore;

namespace WebApplication1.Data.Queries
{
    public class BloggingContextRunner : IRunner<BloggingContext>
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
