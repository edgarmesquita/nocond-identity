using Lamar;
using MediatR;

namespace NoCond.Identity.Web.Registry
{
    public class WebServiceRegistry : ServiceRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiServiceRegistry"/> class.
        /// </summary>
        public WebServiceRegistry ()
        {

            Scan (scanner =>
            {
                scanner.AssembliesFromApplicationBaseDirectory ();
                scanner.ConnectImplementationsToTypesClosing (typeof (IRequestHandler<,>));
                scanner.ConnectImplementationsToTypesClosing (typeof (INotificationHandler<>));
            });

            For<IMediator> ().Use<Mediator> ().Transient ();
            For<ServiceFactory> ().Use (ctx => ctx.GetInstance);
        }
    }
}