using AJobBoard.Models.DTO;
using AJobBoard.Utils.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AJobBoard.Services
{
    public class NLTKService : INLTKService
    {
        private readonly string _nltkSecretKey;
        private readonly string _urlflask;
        private readonly string _GetNLTKKeyPhrases;
        private readonly string _GetNLTKSummary;
        private readonly string applicationJson = "application/json";
        private readonly IHttpClientFactory _clientFactory;
        private readonly AsyncRetryPolicy<KeyPhrasesWrapperDTO> _retryPolicyKeyPhrases;
        private readonly AsyncRetryPolicy<SummaryDTO> _retryPolicySummary;

        public NLTKService(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _nltkSecretKey = Secrets.getAppSettingsValue(configuration, "Auth-FlaskNLTK");
            _urlflask = Secrets.getAppSettingsValue(configuration, "FlaskNLTK-Prod");
            _GetNLTKKeyPhrases = Secrets.getAppSettingsValue(configuration, "_GetNLTKKeyPhrases");
            _GetNLTKSummary = Secrets.getAppSettingsValue(configuration, "_GetNLTKSummary");
            _clientFactory = clientFactory;

            _retryPolicyKeyPhrases = Policy<KeyPhrasesWrapperDTO>
               .Handle<HttpRequestException>().RetryAsync(
               Int32.Parse(Secrets.getAppSettingsValue(configuration, "MaxRetry")));

            _retryPolicySummary = Policy<SummaryDTO>
               .Handle<HttpRequestException>().RetryAsync(
               Int32.Parse(Secrets.getAppSettingsValue(configuration, "MaxRetry")));
        }

        public async Task<KeyPhrasesWrapperDTO> GetNLTKKeyPhrases(string Description)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
               _urlflask + _GetNLTKKeyPhrases);

            var client = _clientFactory.CreateClient("NLTK");

            return await _retryPolicyKeyPhrases.ExecuteAsync(async () =>
            {
                var json = JsonConvert.SerializeObject(new
                {
                    textIn = Description,
                    authKey = _nltkSecretKey
                });

                var data = new StringContent(json, Encoding.UTF8, applicationJson);

                var response = await client.PostAsync(_urlflask + _GetNLTKKeyPhrases, data);

                var result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<KeyPhrasesWrapperDTO>(result);
            });
        }

        public async Task<SummaryDTO> GetNLTKSummary(string description)
        {
            description = description.Replace("\"", @"");
           

            var client = _clientFactory.CreateClient("NLTK");

            return await _retryPolicySummary.ExecuteAsync(async () =>
            {
                var json = JsonConvert.SerializeObject(new
                {
                    textIn = description,
                    authKey = _nltkSecretKey
                });
                var data = new StringContent(json, Encoding.UTF8, applicationJson);
                var response = await client.PostAsync(_urlflask + _GetNLTKSummary, data);
                var result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<SummaryDTO>(result);
            });

        }

    }
}
