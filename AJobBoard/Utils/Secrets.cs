using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AJobBoard.Utils
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
            }

            return Environment.GetEnvironmentVariable(name);
        }


    }
}
