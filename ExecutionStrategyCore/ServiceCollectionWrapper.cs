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

        public void AddSingleton<T>(T implementationInstance)
        where T : class
        {
            collection.AddSingleton<T>(implementationInstance);
        }

        public void AddScoped<T, T2>()
            where T : class
            where T2 : class, T
        {
            collection.AddScoped<T, T2>();
        }
    }
}
