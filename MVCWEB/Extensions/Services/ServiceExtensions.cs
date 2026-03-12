using Microsoft.Extensions.DependencyInjection;
using MVCWEB.Data;
using MVCWEB.Services.Abstract;
using MVCWEB.Services;

namespace MVCWEB.Extensions.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
            
            // 
            services.AddSingleton<DapperContext>();
            services.AddSignalR();

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}
