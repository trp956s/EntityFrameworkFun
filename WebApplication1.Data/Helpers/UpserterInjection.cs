using WebApplication1.Data.Core;

namespace WebApplication1.Data.Helpers
{
    public class UpserterInjection<T> : IDependencyInjectionWrapper<IUpsertDbSet<T>>
    where T : class
    {
        public UpserterInjection(IUpsertDbSet<T> blogContext)
        {
            Dependency = blogContext;
        }

        public IUpsertDbSet<T> Dependency { get; }
    }
}
