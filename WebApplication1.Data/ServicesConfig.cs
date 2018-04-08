using ExecutionStrategyCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class ServicesConfig : IServicesConfig
    {
        private readonly IServiceCollectionWrapper serviceCollectionWrapper;

        public ServicesConfig(IServiceCollectionWrapper serviceCollectionWrapper)
        {
            this.serviceCollectionWrapper = serviceCollectionWrapper;
        }

        public void ConfigureServices()
        {
            var connection = @"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.AspNetCore.NewDb;Trusted_Connection=True;ConnectRetryCount=0";

            serviceCollectionWrapper.AddDbContext<BloggingContext>(options => options.UseSqlServer(connection));
        }
    }
}
