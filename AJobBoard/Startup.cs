using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Services;
using AJobBoard.Utils.Config;
using AJobBoard.Utils.HangFire;
using AutoMapper;
using Hangfire;
using Hangfire.MySql.Core;
using Jobtransparency.Utils.HangFire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;

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
            services.AddAutoMapper(typeof(Startup));
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Secrets.getConnectionString(Configuration, "RedisConnection");
            });

            services.AddDbContext<ApplicationDbContext>(options => options
                // replace with your connection string
                .UseMySql(Secrets.getConnectionString(Configuration, "JobTransparncyDigitalOceanPROD"), mySqlOptions => mySqlOptions
                    // replace with your Server Version and Type
                    .ServerVersion(new ServerVersion(new Version(5, 7, 29), ServerType.MySql))
                    .CommandTimeout(300)
                ));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Password Strength Setting
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            //Setting the Account Login page
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here,
                // ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here,
                // ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings().UseStorage(
                    new MySqlStorage(
                        Secrets.getConnectionString(Configuration, "HangfireConnectionDigitalOceanPROD"),
                        new MySqlStorageOptions
                        {
                            QueuePollInterval = TimeSpan.FromSeconds(15),
                            JobExpirationCheckInterval = TimeSpan.FromHours(1),
                            CountersAggregateInterval = TimeSpan.FromMinutes(5),
                            PrepareSchemaIfNecessary = true,
                            DashboardJobListLimit = 50000,
                            TransactionTimeout = TimeSpan.FromMinutes(1),
                            TablePrefix = "Hangfire"
                        })));


            // Add the processing server as IHostedService
            services.AddHangfireServer();
            services.AddResponseCaching();
            services.AddSession();
            services.AddMvc();
            services.AddRazorPages(); //.AddRazorRuntimeCompilation();

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("AuthKey", policy =>
                //    policy.Requirements.Add(new HasAuthKey(Configuration)));
                options.AddPolicy("CanCreatePosting", policy => policy.RequireClaim("CanCreatePosting"));
                options.AddPolicy("CanEditPosting", policy => policy.RequireClaim("CanEditPosting"));
                options.AddPolicy("CanDeletePosting", policy => policy.RequireClaim("CanDeletePosting"));

            });

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddTransient<UserManager<ApplicationUser>>();

            services.AddTransient<IJobPostingRepository, JobPostingRepository>();
            services.AddTransient<IKeyPharseRepository, KeyPharseRepository>();
            services.AddTransient<IAppliesRepository, AppliesRepository>();
            services.AddTransient<IDocumentRepository, DocumentRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddScoped<IMyJob, KeyPhraseGeneratorJob>();
            services.AddScoped<IMyJob, SummaryGeneratorJob>();

            services.AddSingleton<IAWSService, AWSService>();
            services.AddSingleton<ElasticService, ElasticService>();
            services.AddSingleton<INLTKService, NLTKService>();

            services.AddResponseCompression();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            });
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 2,
            });

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });


            HangFireJobScheduler.ScheduleRecurringJobs();

        }
    }
}