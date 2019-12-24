﻿using AJobBoard.Areas.Identity.Pages.Account;
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

namespace AJobBoard.Services
{
    public class NLTKService
    {
        private string NLTKSecretKey = "";

        public NLTKService(IConfiguration configuration)
        {
            NLTKSecretKey = configuration.GetSection("AppSettings")["Auth-Classify"];
        }

        public async Task<SummaryDataWrapperDTO> GetNLTKSummary(string Description)
        {
            var json = JsonConvert.SerializeObject(new
            {
                textIn = Description,
                authKey = NLTKSecretKey
            });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://127.0.0.1:5000/Classify";
            var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;

            SummaryDataWrapperDTO list = JsonConvert
                .DeserializeObject<SummaryDataWrapperDTO>(result);

            return list;
        }

    }
}