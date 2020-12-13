using AJobBoard;
using AJobBoard.Data;
using AJobBoard.Models.Entity;
using AJobBoard.Services;
using AJobBoard.Utils.Config;
using AJobBoard.Utils.HangFire;
using AutoMapper;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Jobtransparency
{
    public static class ConfigureServicesExtentions
    {
        public static void UseAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
        }
        public static void UseHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient("NLTK");
        }
        public static void UseInjectables(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddTransient<UserManager<ApplicationUser>>();

            services.AddTransient<IJobPostingRepository, JobPostingRepository>();
            services.AddTransient<IKeyPharseRepository, KeyPharseRepository>();
            services.AddTransient<IAppliesRepository, AppliesRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddScoped<ICustomJob, KeyPhraseGeneratorJob>();
            services.AddScoped<ICustomJob, SummaryGeneratorJob>();

            services.AddSingleton<ElasticService, ElasticService>();
            services.AddSingleton<INLTKService, NLTKService>();
        }

        public static void UseIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Password Strength Setting
            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;

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

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
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

        }

        public static void UseAuthorizationRules(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                //options.AddPolicy("AuthKey", policy =>
                //    policy.Requirements.Add(new HasAuthKey(Configuration)));
                options.AddPolicy("CanCreatePosting", policy => policy.RequireClaim("CanCreatePosting"));
                options.AddPolicy("CanEditPosting", policy => policy.RequireClaim("CanEditPosting"));
                options.AddPolicy("CanDeletePosting", policy => policy.RequireClaim("CanDeletePosting"));

            });
        }

        public static void UseMVC(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddResponseCaching();
            services.AddMvc();
            if (Configuration.GetValue<string>("Environment").Equals("Dev"))
            {
                services.AddRazorPages().AddRazorRuntimeCompilation();
            }
            else
            {
                services.AddRazorPages();
            }
            services.AddResponseCompression();

        }

        public static void UseDataStores(this IServiceCollection services, IConfiguration Configuration)
        {
            string AppDBConnectionString = "";
            string AppCacheConnectionString = "";

            if (Configuration.GetValue<string>("Environment").Equals("Dev"))
            {
                AppDBConnectionString = Secrets.getConnectionString(Configuration, "JobTransparncy_DB_LOCAL");
                AppCacheConnectionString = Secrets.getConnectionString(Configuration, "RedisConnection_LOCAL");
            }
            else
            {
                AppDBConnectionString = Secrets.getConnectionString(Configuration, "JobTransparncy_DB_PROD");
                AppCacheConnectionString = Secrets.getConnectionString(Configuration, "RedisConnection_PROD");
            }

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = AppCacheConnectionString;
            });


            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseNpgsql(AppDBConnectionString));

            services.AddHangfire(config =>
                 config.UsePostgreSqlStorage(AppDBConnectionString));
        }
    }
}
