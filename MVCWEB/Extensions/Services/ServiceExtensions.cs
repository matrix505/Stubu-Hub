using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using MVCWEB.Config;
using MVCWEB.DAL;
using MVCWEB.DAL.Abstract;
using MVCWEB.Data;
using MVCWEB.Services.Abstract;
using MVCWEB.Services.Auth;
using MVCWEB.Services.Cache;
using MVCWEB.Services.Email;
using MVCWEB.Services.Handler;

namespace MVCWEB.Extensions.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration) {
            
            // Project services
            services.AddSingleton<DapperContext>(); // sql dapper micro-orm
            services.AddSignalR(); // for real-time data
            services.AddScoped<IPasswordHasher, PasswordHasher>(); // for auth service hasher
        
            services.AddScoped<IEmailSenderService, EmailSenderService>(); // Mail Sender'
            services.AddScoped<ICacheService, CacheService>();

            //Mail config
            services.Configure<MailSettings>(
                configuration.GetSection("MailSettings")
                );
            
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler()); //to handle dateonly format on dapper sql db date type

            services.AddMemoryCache();
            // Authentication config with authorization
            services.
                AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/auth/login";
                    options.LogoutPath = "/auth/logout";
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                }
                );
            services.AddAuthorization();

            return services;
        }
    }
}
