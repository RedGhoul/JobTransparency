using AJobBoard.Models.Dto;
using AJobBoard.Utils.Config;
using Jobtransparency.Models.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
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
        private readonly string _GetNLTKSentiment;
        private readonly string applicationJson = "application/json";
        private readonly IHttpClientFactory _clientFactory;
        private readonly AsyncRetryPolicy<KeyPhrasesWrapperDTO> _retryPolicyKeyPhrases;
        private readonly AsyncRetryPolicy<SummaryDTO> _retryPolicySummary;
        private readonly ILogger<NLTKService> _Logger;

        public NLTKService(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<NLTKService> logger)
        {
            _nltkSecretKey = Secrets.GetAppSettingsValue(configuration, "Auth-FlaskNLTK");
            _urlflask = Secrets.GetAppSettingsValue(configuration, "FlaskNLTK-Prod");
            _GetNLTKKeyPhrases = Secrets.GetAppSettingsValue(configuration, "_GetNLTKKeyPhrases");
            _GetNLTKSummary = Secrets.GetAppSettingsValue(configuration, "_GetNLTKSummary");
            _GetNLTKSentiment = Secrets.GetAppSettingsValue(configuration, "_GetNLTKSentiment");
            _clientFactory = clientFactory;

            _retryPolicyKeyPhrases = Policy<KeyPhrasesWrapperDTO>
               .Handle<HttpRequestException>().RetryAsync(
               Int32.Parse(Secrets.GetAppSettingsValue(configuration, "MaxRetry")));

            _retryPolicySummary = Policy<SummaryDTO>
               .Handle<HttpRequestException>().RetryAsync(
               Int32.Parse(Secrets.GetAppSettingsValue(configuration, "MaxRetry")));

            _Logger = logger;
        }

        public async Task<KeyPhrasesWrapperDTO> ExtractKeyPhrases(string Description)
        {
            _Logger.LogInformation($"Sending the following Description {Description} to NLTK Service GetNLTKKeyPhrases");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,
               _urlflask + _GetNLTKKeyPhrases);

            HttpClient client = _clientFactory.CreateClient("NLTK");
            client.Timeout = TimeSpan.FromMinutes(20);
            return await _retryPolicyKeyPhrases.ExecuteAsync(async () =>
            {
                string json = JsonConvert.SerializeObject(new
                {
                    data = Description,
                    authKey = _nltkSecretKey
                });
                _Logger.LogInformation($"Sending the following Payload {json} to NLTK Service GetNLTKKeyPhrases");

                StringContent data = new StringContent(json, Encoding.UTF8, applicationJson);
                HttpResponseMessage response = null;
                try
                {
                    response = await client.PostAsync(_urlflask + _GetNLTKKeyPhrases, data);
                    string result = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<KeyPhrasesWrapperDTO>(result);
                }
                catch (Exception ex)
                {
                    _Logger.LogError($"Following Error occured Message {ex.Message} GetNLTKKeyPhrases");
                    _Logger.LogError($"Following Error occured StackTrace {ex.StackTrace} GetNLTKKeyPhrases");
                    _Logger.LogError($"Following Error occured InnerException {ex.InnerException} GetNLTKKeyPhrases");

                }

             
                return new KeyPhrasesWrapperDTO()
                {
                    rank_list = new List<KeyPhraseDTO>()
                };
            });
        }

        public async Task<SentimentDTO> ExtractSentiment(string Description)
        {
            _Logger.LogInformation($"Sending the following Description {Description} to NLTK Service ExtractSentiment");
            HttpClient client = _clientFactory.CreateClient("NLTK");
            client.Timeout = TimeSpan.FromMinutes(20);
            string json = JsonConvert.SerializeObject(new
            {
                data = Description
            });
            _Logger.LogInformation($"Sending the following Payload {json} to NLTK Service ExtractSentiment");

            StringContent data = new(json, Encoding.UTF8, applicationJson);

            HttpResponseMessage response = null;
            try
            {
                response = await client.PostAsync(_urlflask + _GetNLTKSentiment, data);
                string result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<SentimentDTO>(result);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Following Error occured Message {ex.Message} ExtractSentiment");
                _Logger.LogError($"Following Error occured StackTrace {ex.StackTrace} ExtractSentiment");
                _Logger.LogError($"Following Error occured InnerException {ex.InnerException} ExtractSentiment");

            }
            return new SentimentDTO();
        }

        public async Task<SummaryDTO> ExtractSummary(string Description)
        {
            _Logger.LogInformation($"Sending the following Description {Description} to NLTK Service GetNLTKSummary");

            HttpClient client = _clientFactory.CreateClient("NLTK");
            client.Timeout = TimeSpan.FromMinutes(20);

            string json = JsonConvert.SerializeObject(new
            {
                data = Description
            });
            _Logger.LogInformation($"Sending the following Payload {json} to NLTK Service GetNLTKSummary");

            StringContent data = new StringContent(json, Encoding.UTF8, applicationJson);

            HttpResponseMessage response = null;

            try
            {
                response = await client.PostAsync(_urlflask + _GetNLTKSummary, data);
                string result = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<SummaryDTO>(result);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Following Error occured Message {ex.Message} GetNLTKSummary");
                _Logger.LogError($"Following Error occured StackTrace {ex.StackTrace} GetNLTKSummary");
                _Logger.LogError($"Following Error occured InnerException {ex.InnerException} GetNLTKSummary");
            }
           

            return new SummaryDTO()
            {
                SummaryText = ""
            };
        }

    }
}
