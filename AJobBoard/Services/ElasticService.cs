using AJobBoard.Models;
using AJobBoard.Models.DTO;
using AJobBoard.Utils.Config;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AJobBoard.Services
{
    public class ElasticService
    {

        private static string baseUrlsearch;
        private static ElasticClient elasticClient;

        public ElasticService(IConfiguration configuration)
        {
            baseUrlsearch = Secrets.getConnectionString(configuration, "ElasticIndexBaseUrl");
            var settings = new ConnectionSettings(new Uri(baseUrlsearch))
                .DefaultIndex("jobpostings")
                .BasicAuthentication(
                    Secrets.getAppSettingsValue(configuration, "ELASTIC_USERNAME_Search"),
                    Secrets.getAppSettingsValue(configuration, "ELASTIC_PASSWORD_Search"))
                .RequestTimeout(TimeSpan.FromMinutes(2));
            elasticClient = new ElasticClient(settings);

        }

        public async Task<List<JobPostingDTO>> QueryJobPosting(int fromNumber, string keywords)
        {
            var searchResponse = await elasticClient.SearchAsync<JobPostingDTO>(s => s
                .From(fromNumber)
                .Size(12)
                .Query(q => q
                     .Match(m => m
                        .Field(f => f.Description)
                        .Query(keywords)
                     )
                ).Query(q => q
                     .Match(m => m
                        .Field(f => f.Title)
                        .Query(keywords)
                     )
                ).Sort(q => q.Descending(u => u.DateAdded))
            );

            var JobPosting = searchResponse.Documents;
            return (List<JobPostingDTO>)JobPosting;

            //var client = new HttpClient();

            //string finalQueryString = "";
            //if (string.IsNullOrEmpty(keywords))
            //{
            //    finalQueryString = baseUrlsearch + "/jobpostings/_search?q=from=" + fromNumber + "&size=" + 12 + "&sort=DateAdded:desc";
            //}
            //else
            //{
            //    finalQueryString = baseUrlsearch + "/jobpostings/_search?q=Description:" + keywords + "&from=" + fromNumber + "&size=" + 12 + "&sort=DateAdded:desc";
            //}
            //response = await client.GetAsync(finalQueryString);

            //var result = response.Content.ReadAsStringAsync().Result;

            //try
            //{
            //    RootObject list = JsonConvert
            //        .DeserializeObject<RootObject>(result);
            //    List<JobPostingDTO> listsJobPostings = new List<JobPostingDTO>();
            //    foreach (var item in list.hits.hits)
            //    {

            //        listsJobPostings.Add(item._source);
            //    }
            //    return listsJobPostings;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.InnerException);
            //    return new List<JobPostingDTO>();
            //}
        }

        public async Task<bool> DeleteJobPostingIndexAsync()
        {
            var response = await new HttpClient().DeleteAsync(baseUrlsearch + "/jobpostings");
            return response.IsSuccessStatusCode;
        }

        public async Task CreateJobPostingAsync(JobPostingDTO jobPosting)
        {
            var things = await elasticClient.IndexDocumentAsync(jobPosting);

            //var json = JsonConvert.SerializeObject(jobPosting, Formatting.None,
            //    new JsonSerializerSettings()
            //    {
            //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //    });
            //var data = new StringContent(json, Encoding.UTF8, "application/json");

            //var client = new HttpClient();

            //var response = await client.PutAsync(baseUrlsearch + "/jobpostings/_doc/" + jobPosting.Id, data);
        }



        public async Task<List<JobPostingDTO>> GetRandomSetOfJobPosting()
        {
            var searchResponse = await elasticClient.SearchAsync<JobPostingDTO>(
                s => s.Size(12)
                .Sort(q => q.Descending(u => u.DateAdded)
                ));

            var JobPosting = searchResponse.Documents;
            return (List<JobPostingDTO>)JobPosting;


            //var client = new HttpClient();

            //var response = await client.GetAsync(baseUrlsearch +"/jobpostings/_search?" +"&from=" + new Random().Next(1,12) + "&size=" + 12 + "&sort=DateAdded:desc");

            //var result = response.Content.ReadAsStringAsync().Result;

            //try
            //{
            //    RootObject list = JsonConvert
            //        .DeserializeObject<RootObject>(result);
            //    List<JobPostingDTO> listsJobPostings = new List<JobPostingDTO>();
            //    foreach (var item in list.hits.hits)
            //    {

            //        listsJobPostings.Add(item._source);
            //    }
            //    return listsJobPostings;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.InnerException);
            //    return new List<JobPostingDTO>();
            //}

        }

        public async Task CreateJobPostingBulk(List<JobPosting> jobPostings)
        {

        }


        public void UpdateJobPosting(JobPosting jobPosting)
        {


        }
    }
}
