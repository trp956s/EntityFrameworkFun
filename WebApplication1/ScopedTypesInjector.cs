using ExecutionStrategyCore;
using WebApplication1.Data;

namespace WebApplication1
{
    public class ScopedTypesInjector : IServicesConfig
    {
        private readonly IServiceCollectionWrapper scopedServicesWrapper;

        public ScopedTypesInjector(IServiceCollectionWrapper scopedServicesWrapper)
        {
            this.scopedServicesWrapper = scopedServicesWrapper;
        }

        public void ConfigureServices()
        {
            scopedServicesWrapper.AddConfig<ServicesConfig>();
        }
    }
}
