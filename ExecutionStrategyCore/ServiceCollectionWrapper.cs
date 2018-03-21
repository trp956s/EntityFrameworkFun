using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;


namespace ExecutionStrategyCore
{
    public class ServiceCollectionWrapper : IServiceCollectionWrapper
    {
        private readonly IServiceCollection collection;

        public ServiceCollectionWrapper(IServiceCollection collection)
        {
            this.collection = collection;
        }

        public void AddConfig<T>()
        where T : class, IServicesConfig
        {
            AddScoped<T>();
            var config = collection.BuildServiceProvider().GetService<T>();
            config.ConfigureServices();
        }

        public void AddScoped<T>()
        where T : class
        {
            collection.AddScoped<T>();
        }

        public void AddDbContext<T>(Action<DbContextOptionsBuilder> optionsAction)
        where T : DbContext
        {
            collection.AddDbContext<T>(optionsAction);
        }
    }
}
