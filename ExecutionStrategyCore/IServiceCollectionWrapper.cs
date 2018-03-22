using System;
using Microsoft.EntityFrameworkCore;

namespace ExecutionStrategyCore
{
    public interface IServiceCollectionWrapper
    {
        void AddConfig<T>() where T : class, IServicesConfig;
        void AddDbContext<T>(Action<DbContextOptionsBuilder> optionsAction) where T : DbContext;
        void AddScoped<T>() where T : class;
        void AddScoped<T, T2>() where T : class where T2 : class, T;
        void AddSingleton<T>(T implementationInstance) where T : class;
    }
}