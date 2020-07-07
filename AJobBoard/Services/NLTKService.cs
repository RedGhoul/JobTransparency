using AJobBoard.Models.DTO;
using AJobBoard.Utils.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

        public NLTKService(IConfiguration configuration)
        {
            _nltkSecretKey = Secrets.getAppSettingsValue(configuration, "Auth-FlaskNLTK");
            _urlflask = Secrets.getAppSettingsValue(configuration, "FlaskNLTK-Prod");
            _GetNLTKKeyPhrases = Secrets.getAppSettingsValue(configuration, "_GetNLTKKeyPhrases");
            _GetNLTKSummary = Secrets.getAppSettingsValue(configuration, "_GetNLTKSummary");
        }

        public async Task<KeyPhrasesWrapperDTO> GetNLTKKeyPhrases(string Description)
        {
            var json = JsonConvert.SerializeObject(new
            {
                textIn = Description,
                authKey = _nltkSecretKey
            });

            var data = new StringContent(json, Encoding.UTF8, applicationJson);

            var client = new HttpClient();

            var response = await client.PostAsync(_urlflask + _GetNLTKKeyPhrases, data);

            string result = response.Content.ReadAsStringAsync().Result;

            try
            {
                KeyPhrasesWrapperDTO list = JsonConvert
                .DeserializeObject<KeyPhrasesWrapperDTO>(result);
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<SummaryDTO> GetNLTKSummary(string description)
        {
            description = description.Replace("\"", @"");
            var json = JsonConvert.SerializeObject(new
            {
                textIn = description,
                authKey = _nltkSecretKey
            });
            var data = new StringContent(json, Encoding.UTF8, applicationJson);
            Console.WriteLine(data);
            var client = new HttpClient();

            var response = await client.PostAsync(_urlflask + _GetNLTKSummary, data);

            string result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);
            try
            {
                SummaryDTO list = JsonConvert
               .DeserializeObject<SummaryDTO>(result);
                return list;
            }
            catch (Exception)
            {
                return null;
            }

        }

    }
}
