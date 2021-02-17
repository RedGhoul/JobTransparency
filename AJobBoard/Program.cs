using AJobBoard.Utils.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Sentry;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;

namespace AJobBoard
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            IConfiguration configuration = null;
            try
            {
                configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();
            }
            catch (Exception e)
            {
                configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
                Console.WriteLine(e);
            }


            string AppDBConnectionString = Secrets.GetDBConnectionString(configuration);

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                 .WriteTo.MySQL(AppDBConnectionString)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }


        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
        }
    }
}
