using System.Reflection;
using eQuantic.Core.Data.Repository;
using Lamar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NoCond.Identity.Persistence.Registry
{
    public class PersistenceServiceRegistry : ServiceRegistry
    {
        public PersistenceServiceRegistry ()
        {
            For<DbContext> ().Use (c => c.GetInstance<IdentityContext> ());
            For<IIdentityUnitOfWork> ().Use<IdentityUnitOfWork> ().Transient ();

            Scan (scanner =>
            {
                scanner.AssembliesAndExecutablesFromApplicationBaseDirectory ();
                scanner.Include (
                    t =>
                    !t.GetTypeInfo ().IsAbstract && !t.GetTypeInfo ().IsGenericTypeDefinition &&
                    typeof (IRepository).IsAssignableFrom (t));

                scanner.WithDefaultConventions (ServiceLifetime.Transient);
            });
        }
    }
}