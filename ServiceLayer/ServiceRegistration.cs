using DataAccessLayer;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceLayer
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            services.AddTransient<IService, Service>();
            services.AddTransient<IDataRepository, DataRepository>();

            return services;
        }
    }
}