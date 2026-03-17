using MVCWEB.DAL;
using MVCWEB.DAL.Abstract;

namespace MVCWEB.Extensions.Services
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IUsersRepository, UserRepository>(); // for user client dal
            services.AddScoped<IAccountRepository, AccountRepository>(); // for authentication dal
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IDiscussionRepository, DiscussionRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            //

            return services;
        }
    }
}
