using AJobBoard.Areas.Identity.Pages.Account;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using AJobBoard.Models.Data;
using AJobBoard.Models.DTO;
using AJobBoard.Utils;

namespace AJobBoard.Services
{
    public class NLTKService : INLTKService
    {
        private string NLTKSecretKey = "";
        private string URLFLASK = "";
        public NLTKService(IConfiguration configuration)
        {
            NLTKSecretKey = Secrets.getAppSettingsValue(configuration, "Auth-FlaskNLTK");
            URLFLASK = Secrets.getAppSettingsValue(configuration, "FlaskNLTK-Prod");
        }

        public async Task<KeyPhrasesWrapperDTO> GetNLTKKeyPhrases(string Description)
        {
            var json = JsonConvert.SerializeObject(new
            {
                textIn = Description,
                authKey = NLTKSecretKey
            });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            var response = await client.PostAsync(URLFLASK + "/extract_keyphrases_from_text", data);

            string result = response.Content.ReadAsStringAsync().Result;

            try
            {
                KeyPhrasesWrapperDTO list = JsonConvert
                .DeserializeObject<KeyPhrasesWrapperDTO>(result);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<SummaryDTO> GetNLTKSummary(string Description)
        {
            var json = JsonConvert.SerializeObject(new
            {
                textIn = Description,
                authKey = NLTKSecretKey
            });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            var response = await client.PostAsync(URLFLASK + "/extract_summary_from_text", data);

            string result = response.Content.ReadAsStringAsync().Result;

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
