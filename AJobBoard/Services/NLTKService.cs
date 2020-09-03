using AJobBoard.Models.DTO;
using AJobBoard.Utils.Config;
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
        private readonly string applicationJson = "application/json";
        private readonly IHttpClientFactory _clientFactory;
        private readonly AsyncRetryPolicy<KeyPhrasesWrapperDTO> _retryPolicyKeyPhrases;
        private readonly AsyncRetryPolicy<SummaryDTO> _retryPolicySummary;
        private readonly ILogger<NLTKService> _Logger;

        public NLTKService(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<NLTKService> logger)
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

            _Logger = logger;
        }

        public async Task<KeyPhrasesWrapperDTO> GetNLTKKeyPhrases(string Description)
        {
            _Logger.LogInformation($"Sending the following Description {Description} to NLTK Service GetNLTKKeyPhrases");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,
               _urlflask + _GetNLTKKeyPhrases);

            HttpClient client = _clientFactory.CreateClient("NLTK");

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
                }
                catch (Exception ex)
                {
                    _Logger.LogError($"Following Error occured Message {ex.Message} GetNLTKKeyPhrases");
                    _Logger.LogError($"Following Error occured StackTrace {ex.StackTrace} GetNLTKKeyPhrases");
                    _Logger.LogError($"Following Error occured InnerException {ex.InnerException} GetNLTKKeyPhrases");

                }

                string result = response.Content.ReadAsStringAsync().Result;

                try
                {
                    return JsonConvert.DeserializeObject<KeyPhrasesWrapperDTO>(result);
                }
                catch (Exception ex)
                {
                    _Logger.LogError($"Following DeserializeObject Error occured content of result {result} GetNLTKKeyPhrases");
                    _Logger.LogError($"Following DeserializeObject Error occured Message {ex.Message} GetNLTKKeyPhrases");
                    _Logger.LogError($"Following DeserializeObject Error occured StackTrace {ex.StackTrace} GetNLTKKeyPhrases");
                    _Logger.LogError($"Following DeserializeObject Error occured InnerException {ex.InnerException} GetNLTKKeyPhrases");

                }
                return new KeyPhrasesWrapperDTO()
                {
                    rank_list = new List<KeyPhraseDTO>()
                };
            });
        }

        public async Task<SummaryDTO> GetNLTKSummary(string description)
        {
            _Logger.LogInformation($"Sending the following Description {description} to NLTK Service GetNLTKSummary");

            HttpClient client = _clientFactory.CreateClient("NLTK");

            return await _retryPolicySummary.ExecuteAsync(async () =>
            {
                string json = JsonConvert.SerializeObject(new
                {
                    data = description,
                    authKey = _nltkSecretKey
                });
                _Logger.LogInformation($"Sending the following Payload {json} to NLTK Service GetNLTKSummary");

                StringContent data = new StringContent(json, Encoding.UTF8, applicationJson);

                HttpResponseMessage response = null;
                try
                {
                    response = await client.PostAsync(_urlflask + _GetNLTKSummary, data);
                }
                catch (Exception ex)
                {
                    _Logger.LogError($"Following Error occured Message {ex.Message} GetNLTKSummary");
                    _Logger.LogError($"Following Error occured StackTrace {ex.StackTrace} GetNLTKSummary");
                    _Logger.LogError($"Following Error occured InnerException {ex.InnerException} GetNLTKSummary");

                }

                string result = response.Content.ReadAsStringAsync().Result;

                try
                {
                    return JsonConvert.DeserializeObject<SummaryDTO>(result);
                }
                catch (Exception ex)
                {
                    _Logger.LogError($"Following DeserializeObject Error occured content of result {result} GetNLTKSummary");
                    _Logger.LogError($"Following DeserializeObject Error occured Message {ex.Message} GetNLTKSummary");
                    _Logger.LogError($"Following DeserializeObject Error occured StackTrace {ex.StackTrace} GetNLTKSummary");
                    _Logger.LogError($"Following DeserializeObject Error occured InnerException {ex.InnerException} GetNLTKSummary");

                }
                return new SummaryDTO()
                {
                    SummaryText = ""
                };
            });

        }

    }
}
