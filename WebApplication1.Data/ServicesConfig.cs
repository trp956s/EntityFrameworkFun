using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication1.Data
{
    public class ServicesConfig
    {
        public void Configure(IServiceCollection services, string connection)
        {
            services.AddDbContext<BloggingContext>(options => options.UseSqlServer(connection));
            services.AddScoped<IAsyncExecutableRunner, AsyncExecutableRunner>();
        }
    }
}
