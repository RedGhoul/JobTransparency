using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Services;
using AJobBoard.Utils;
using AJobBoard.Utils.AuthorizationHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Hangfire;
using Hangfire.MySql.Core;
using Hangfire.Dashboard;

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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration.GetConnectionString("RedisConnection");
            });

            services.AddDbContext<ApplicationDbContext>(options => 
               options.UseMySql(
                   Configuration.GetConnectionString("JobTransparncyDigitalOceanPROD")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
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



            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore); ;

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AuthKey", policy =>
                    policy.Requirements.Add(new HasAuthKey(Configuration)));
                options.AddPolicy("CanCreatePosting", policy => policy.RequireClaim("CanCreatePosting"));
                options.AddPolicy("CanEditPosting", policy => policy.RequireClaim("CanEditPosting"));
                options.AddPolicy("CanDeletePosting", policy => policy.RequireClaim("CanDeletePosting"));

            });

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddScoped<UserManager<ApplicationUser>>();
            
            services.AddScoped<IJobPostingRepository, JobPostingRepository>();
            services.AddScoped<IKeyPharseRepository,KeyPharseRepository>();
            services.AddScoped<IAppliesRepository, AppliesRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            services.AddSingleton<IAWSService, AWSService>();
            services.AddScoped<INLTKService ,NLTKService>();
            services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseStorage(
                        new MySqlStorage(
                            Configuration.GetConnectionString("HangfireConnectionDigitalOceanPROD"),
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


            services.AddHangfireServer();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseAuthentication();
            //UseHangfireDashboardCustom(app);
            app.UseHangfireDashboard("/Jobs", new DashboardOptions()
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                    //routes.MapRoute("CHECKUP", "{controller=JobPostingsAPI}/{action=Check}");
            });



            await CreateUserRoles(app);
            RecurringJob.AddOrUpdate<CacheBuilder>("buildCache-id", x => x.Build(),
                Cron.Hourly, null, "cacheqq");
            
        }


        private static IApplicationBuilder UseHangfireDashboardCustom(IApplicationBuilder app, string pathMatch = "/hangfire", DashboardOptions options = null, JobStorage storage = null)
        {
            var services = app.ApplicationServices;
            storage = storage ?? services.GetRequiredService<JobStorage>();
            options = options ?? services.GetService<DashboardOptions>() ?? new DashboardOptions();
            var routes = app.ApplicationServices.GetRequiredService<RouteCollection>();

            app.Map(new PathString(pathMatch), x =>
                x.UseMiddleware<HangfireDashboardMiddleware>(storage, options, routes));

            return app;
        }


        private async Task CreateUserRoles(IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                //var JobsRepo = scope.ServiceProvider.GetRequiredService<IJobPostingRepository>();
                //await JobsRepo.BuildCache();
                var content = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                IdentityResult roleResult;

                //Adding Admin Role
                var roleCheck = await RoleManager.RoleExistsAsync("Admin");
                if (!roleCheck)
                {
                    //create the roles and seed them to the database
                    roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
                }

                //Assign Admin role to the main User here we have given our newly registered 
                //login id for Admin management
                // Also Assigning them Claims to perform CUD operations
                ApplicationUser user = await UserManager.FindByEmailAsync("avaneesab5@gmail.com");
                if (user != null)
                {
                    var currentUserRoles = await UserManager.GetRolesAsync(user);
                    if (!currentUserRoles.Contains("Admin"))
                    {
                        await UserManager.AddToRoleAsync(user, "Admin");
                    }

                    var currentClaims = await UserManager.GetClaimsAsync(user);
                    if(currentClaims.Count() == 0)
                    {
                        var CanCreatePostingClaim = new Claim("CanCreatePosting", "True");
                        await UserManager.AddClaimAsync(user, CanCreatePostingClaim);

                        var CanEditPostingClaim = new Claim("CanEditPosting", "True");
                        await UserManager.AddClaimAsync(user, CanEditPostingClaim);

                        var CanDeletePostingClaim = new Claim("CanDeletePosting", "True");
                        await UserManager.AddClaimAsync(user, CanDeletePostingClaim);
                    }
                }
            }

        }
    }
}