using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using AJobBoard.Models;
using AJobBoard.Models.DTO;
using AJobBoard.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AJobBoard.Services
{
    public class ElasticService
    {

        private static string baseUrlsearch;

        public ElasticService(IConfiguration configuration)
        {
            baseUrlsearch = Secrets.getConnectionString(configuration,"ElasticIndexBaseUrl");
        }

        public async Task<List<JobPostingDTO>> QueryJobPosting(int fromNumber, string keywords)
        {
            var client = new HttpClient();
            
            HttpResponseMessage response = null;
            string finalQueryString = "";
            if (string.IsNullOrEmpty(keywords))
            {
                finalQueryString = baseUrlsearch + "/jobpostings/_search?q=from=" + fromNumber + "&size=" + 12 + "&sort=DateAdded:desc";
            }
            else
            {
                finalQueryString = baseUrlsearch + "/jobpostings/_search?q=Description:" + keywords + "&from=" + fromNumber + "&size=" + 12 + "&sort=DateAdded:desc";
            }
            response = await client.GetAsync(finalQueryString);

            var result = response.Content.ReadAsStringAsync().Result;
            
            try
            {
                RootObject list = JsonConvert
                    .DeserializeObject<RootObject>(result);
                List<JobPostingDTO> listsJobPostings = new List<JobPostingDTO>();
                foreach (var item in list.hits.hits)
                {
                    
                    listsJobPostings.Add(item._source);
                }
                return listsJobPostings;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return new List<JobPostingDTO>();
            }


        }

        public async Task CreateJobPostingAsync(JobPosting jobPosting)
        {
            var json = JsonConvert.SerializeObject(jobPosting, Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            var response = await client.PutAsync(baseUrlsearch + "/jobpostings/_doc/" + jobPosting.Id, data);
        }

       

        public async Task<List<JobPostingDTO>> GetRandomSetOfJobPosting()
        {
            var client = new HttpClient();
           
            var response = await client.GetAsync(baseUrlsearch +"/jobpostings/_search?" +"&from=" + new Random().Next(1,12) + "&size=" + 12 + "&sort=DateAdded:desc");

            var result = response.Content.ReadAsStringAsync().Result;

            try
            {
                RootObject list = JsonConvert
                    .DeserializeObject<RootObject>(result);
                List<JobPostingDTO> listsJobPostings = new List<JobPostingDTO>();
                foreach (var item in list.hits.hits)
                {

                    listsJobPostings.Add(item._source);
                }
                return listsJobPostings;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return new List<JobPostingDTO>();
            }

        }

        public async Task CreateJobPostingBulk(List<JobPosting> jobPostings)
        {

        }


        public void UpdateJobPosting(JobPosting jobPosting)
        {


        }
    }
}
