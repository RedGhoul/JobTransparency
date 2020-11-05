using AJobBoard.Utils.HangFire;
using AJobBoard.Utils.Seeder;
using Hangfire;
using Jobtransparency.Utils.HangFire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Jobtransparency
{
    public static class ConfigureAppExtentions
    {
        public static void UseBasicConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

        }

        public static void UseAuthConfiguration(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        public static void UseHangFireConfiguration(this IApplicationBuilder app)
        {
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 6,
            });
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            });
        }

        public static void UseEndPointConfiguration(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        public static async Task UseStartUpMethodsAsync(this IApplicationBuilder app)
        {
            await Seeder.CreateUserRoles(app);
            HangFireJobScheduler.ScheduleRecurringJobs();
        }
    }
}
