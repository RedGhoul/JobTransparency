using System;
using AJobBoard.Utils;
using AJobBoard.Utils.Config;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Sinks.Elasticsearch;

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
                Console.WriteLine(e);
            }
            
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                 .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri($"{Secrets.getConnectionString(configuration, "Log_ElasticIndexBaseUrl")}"))
                 {
                     AutoRegisterTemplate = true,
                     ModifyConnectionSettings = x => x.BasicAuthentication(Secrets.getAppSettingsValue(configuration, "elastic_name"), Secrets.getAppSettingsValue(configuration, "elastic_pasword")),
                     AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                     IndexFormat = $"{Secrets.getAppSettingsValue(configuration, "AppName")}" + "-{0:yyyy.MM}"
                 })
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

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
