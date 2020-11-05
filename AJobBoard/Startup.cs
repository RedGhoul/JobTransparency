using Hangfire;
using Jobtransparency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AJobBoard
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.UseAutoMapper();

            services.UseDataStores(Configuration);

            services.UseIdentity();

            services.UseMVC(Configuration);

            services.UseInjectables(Configuration);

            services.UseAuthorizationRules();

            services.UseHttpClient();

            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseBasicConfiguration(env);

            app.UseAuthConfiguration();

            app.UseHangFireConfiguration();

            app.UseEndPointConfiguration();

            await app.UseStartUpMethodsAsync();
        }
    }
}