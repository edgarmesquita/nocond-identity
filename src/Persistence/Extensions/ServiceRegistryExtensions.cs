using Lamar;
using NoCond.Identity.Persistence.Registry;

namespace NoCond.Identity.Persistence.Extensions
{
    public static class ServiceRegistryExtensions
    {
        /// <summary>
        /// Includes the persistence service registry.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static ServiceRegistry IncludePersistenceServiceRegistry (this ServiceRegistry services)
        {
            services.IncludeRegistry<PersistenceServiceRegistry> ();
            return services;
        }
    }
}