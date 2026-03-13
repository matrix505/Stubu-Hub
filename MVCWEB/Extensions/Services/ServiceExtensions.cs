using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using MVCWEB.Data;
using MVCWEB.Services;
using MVCWEB.Services.Abstract;

namespace MVCWEB.Extensions.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
            
            // 
            services.AddSingleton<DapperContext>();
            services.AddSignalR();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // Authentication config with authorization
            services.
                AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/auth/login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = true;
                }
                );
            services.AddAuthorization();

            return services;
        }
    }
}
