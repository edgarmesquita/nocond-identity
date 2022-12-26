using Lamar;
using Microsoft.AspNetCore.Identity.UI.Services;
using NoCond.Identity.Infrastructure.Sender;

namespace NoCond.Identity.Infrastructure.Registry
{
    public class InfrastructureServiceRegistry : ServiceRegistry
    {
        public InfrastructureServiceRegistry()
        {
            For<IEmailSender>().Use<EmailSender>();
        }
    }
}