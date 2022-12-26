using Lamar;
using NoCond.Identity.Application.Registry;

namespace NoCond.Identity.Application.Extensions
{
    public static class ServiceRegistryExtensions
    {
        /// <summary>
        /// Includes the application service registry.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static ServiceRegistry IncludeApplicationServiceRegistry (this ServiceRegistry services)
        {
            services.IncludeRegistry<ApplicationServiceRegistry> ();
            return services;
        }
    }
}