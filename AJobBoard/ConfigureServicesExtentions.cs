using AJobBoard;
using AJobBoard.Data;
using AJobBoard.Services;
using AJobBoard.Utils.Config;
using AJobBoard.Utils.HangFire;
using AutoMapper;
using Hangfire;
using Hangfire.MySql.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
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
            services.AddTransient<IDocumentRepository, DocumentRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddScoped<ICustomJob, KeyPhraseGeneratorJob>();
            services.AddScoped<ICustomJob, SummaryGeneratorJob>();

            services.AddSingleton<IAWSService, AWSService>();
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
            string AppHangFireConnectionString = "";

            if (Configuration.GetValue<string>("Environment").Equals("Dev"))
            {
                AppDBConnectionString = Secrets.getConnectionString(Configuration, "JobTransparncy_DB_LOCAL");
                AppCacheConnectionString = Secrets.getConnectionString(Configuration, "RedisConnection_LOCAL");
                AppHangFireConnectionString = Secrets.getConnectionString(Configuration, "Hangfire_DB_LOCAL");
            }
            else
            {
                AppDBConnectionString = Secrets.getConnectionString(Configuration, "JobTransparncy_DB_PROD");
                AppCacheConnectionString = Secrets.getConnectionString(Configuration, "RedisConnection_PROD");
                AppHangFireConnectionString = Secrets.getConnectionString(Configuration, "Hangfire_DB_PROD");
            }

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = AppCacheConnectionString;
            });


            services.AddDbContext<ApplicationDbContext>(options => options
                .UseMySql(AppDBConnectionString, mySqlOptions => mySqlOptions
                    .ServerVersion(new ServerVersion(new Version(8, 0, 19), ServerType.MySql))
                    .CommandTimeout(300)
                ));

            services.AddHangfire(configuration => configuration
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings().UseStorage(
                   new MySqlStorage(AppHangFireConnectionString,
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
        }
    }
}
