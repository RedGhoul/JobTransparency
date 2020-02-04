using System;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;

namespace AJobBoard.Utils
{
    public class JankySink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly IConfiguration _configurationProvider;

        public JankySink(IFormatProvider formatProvider, IConfiguration configurationProvider)
        {
            _formatProvider = formatProvider;
            _configurationProvider = configurationProvider;
        }

        public async void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            string password = Secrets.getAppSettingsValue(_configurationProvider, "JankAuthPassword");
            var json = JsonConvert.SerializeObject(new { msg = message, KEYAUTH = password }, Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            string postUrl = Secrets.getConnectionString(_configurationProvider, "JankConnectionString");
            if (postUrl != null)
            {
                var response = await client.PostAsync(postUrl, data);
            }

        }
    }
}