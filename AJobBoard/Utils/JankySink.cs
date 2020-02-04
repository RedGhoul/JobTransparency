﻿using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;

namespace AJobBoard.Utils
{
    public class JankySink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;

        public JankySink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public async void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            var json = JsonConvert.SerializeObject(new { msg = message, KEYAUTH = "apples" }, Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            var response = await client.PostAsync("http://app-janky.experimentsinthedeep.com/elasticsearch/log/logss", data);
        }
    }
}