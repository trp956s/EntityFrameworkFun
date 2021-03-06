﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Data.Core;
using WebApplication1.Data.Injectors;

namespace WebApplication1.Data
{
    public class ServicesConfig
    {
        public void Configure(IServiceCollection services, string connection)
        {
            services.AddDbContext<BloggingContext>(options => options.UseSqlServer(connection));
            services.AddScoped<IAsyncExecutableRunner, AsyncExecutableRunner>();
            services.AddScoped<BlogDbSetInjector>();
        }
    }
}
