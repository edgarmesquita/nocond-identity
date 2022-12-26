using Lamar;
using NoCond.Identity.Web.Registry;

namespace NoCond.Identity.Web.Extensions
{
    public static class ServiceRegistryExtensions
    {
        /// <summary>
        /// Includes the web service registry.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static ServiceRegistry IncludeWebServiceRegistry (this ServiceRegistry services)
        {
            services.IncludeRegistry<WebServiceRegistry> ();
            return services;
        }
    }
}