using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(AJobBoard.Areas.Identity.IdentityHostingStartup))]
namespace AJobBoard.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}