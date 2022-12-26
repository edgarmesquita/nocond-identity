using Microsoft.AspNetCore.Hosting;
using NoCond.Identity.Web.Areas.Identity;

[assembly : HostingStartup (typeof (IdentityHostingStartup))]
namespace NoCond.Identity.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure (IWebHostBuilder builder)
        {
            builder.ConfigureServices ((context, services) =>
            { });
        }
    }
}