using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;

namespace AJobBoard.Utils
{
    public static class MySinkExtensions
    {
        public static LoggerConfiguration MySink(
            this LoggerSinkConfiguration loggerConfiguration,
            IConfiguration configuration,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new JankySink(formatProvider, configuration));
        }
    }
}
