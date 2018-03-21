using System;
using Microsoft.EntityFrameworkCore;

namespace ExecutionStrategyCore
{
    public interface IServiceCollectionWrapper
    {
        void AddConfig<T>() where T : class, IServicesConfig;
        void AddDbContext<T>(Action<DbContextOptionsBuilder> optionsAction) where T : DbContext;
        void AddScoped<T>() where T : class;
    }
}