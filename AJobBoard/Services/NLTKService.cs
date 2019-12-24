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

namespace AJobBoard.Services
{
    public class NLTKService
    {
        private string NLTKSecretKey = "";

        public NLTKService(IConfiguration configuration)
        {
            string NLTKSecretKey = configuration.GetSection("AppSettings")["Auth-Classify"];
        }

        public string getSummary(string Description)
        {

            return "";
        }

    }
}
