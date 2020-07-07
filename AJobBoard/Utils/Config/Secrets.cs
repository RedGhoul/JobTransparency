using Microsoft.Extensions.Configuration;
using System;

namespace AJobBoard.Utils.Config
{
    public static class Secrets
    {

        public static string getAppSettingsValue(IConfiguration Configuration, string name)
        {
            try
            {
                var value = Configuration.GetSection("AppSettings")[name];
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

        public static string getConnectionString(IConfiguration Configuration, string name)
        {
            try
            {
                var value = Configuration.GetConnectionString(name);
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


    }
}
