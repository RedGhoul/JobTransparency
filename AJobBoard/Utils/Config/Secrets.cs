using Microsoft.Extensions.Configuration;
using System;

namespace AJobBoard.Utils.Config
{
    public static class Secrets
    {

        public static string GetAppSettingsValue(IConfiguration Configuration, string name)
        {
            try
            {
                string value = Configuration.GetSection("AppSettings")[name];
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Could not find it in the Configuration");
                Console.WriteLine("using the following value instead: " + Environment.GetEnvironmentVariable(name));
            }

            return Environment.GetEnvironmentVariable(name);
        }

        public static string GetConnectionString(IConfiguration Configuration, string name)
        {
            try
            {
                string value = Configuration.GetConnectionString(name);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Could not find it in the Configuration");
                Console.WriteLine("using the following value instead: " + Environment.GetEnvironmentVariable(name));
            }

            return Environment.GetEnvironmentVariable(name);
        }

        public static string GetDBConnectionString(IConfiguration Configuration)
        {
            string AppDBConnectionString = "";
            string AppCacheConnectionString = "";

            if (Configuration.GetValue<string>("Environment").Equals("Dev"))
            {
                AppDBConnectionString = GetConnectionString(Configuration, "JobTransparncy_DB_LOCAL");
                AppCacheConnectionString = GetConnectionString(Configuration, "RedisConnection_LOCAL");
            }
            else
            {
                AppDBConnectionString = GetConnectionString(Configuration, "JobTransparncy_DB_PROD");
                AppCacheConnectionString = GetConnectionString(Configuration, "RedisConnection_PROD");
            }

            return AppDBConnectionString;
        }


    }
}
