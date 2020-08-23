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
            ConnectionSettings settings = new ConnectionSettings(new Uri(baseUrlsearch))
                .DefaultIndex("jobpostings")
                .BasicAuthentication(
                    Secrets.getAppSettingsValue(configuration, "ELASTIC_USERNAME_Search"),
                    Secrets.getAppSettingsValue(configuration, "ELASTIC_PASSWORD_Search"))
                .RequestTimeout(TimeSpan.FromMinutes(2));
            elasticClient = new ElasticClient(settings);

        }

        public async Task<List<JobPostingDTO>> QueryJobPosting(int fromNumber, string keywords)
        {
            ISearchResponse<JobPostingDTO> searchResponse = await elasticClient.SearchAsync<JobPostingDTO>(s => s
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

            IReadOnlyCollection<JobPostingDTO> JobPosting = searchResponse.Documents;
            return (List<JobPostingDTO>)JobPosting;
        }

        public async Task<bool> DeleteJobPostingIndexAsync()
        {
            HttpResponseMessage response = await new HttpClient().DeleteAsync(baseUrlsearch + "/jobpostings");
            return response.IsSuccessStatusCode;
        }

        public async Task CreateJobPostingAsync(JobPostingDTO jobPosting)
        {
            IndexResponse things = await elasticClient.IndexDocumentAsync(jobPosting);
        }



        public async Task<List<JobPostingDTO>> GetRandomSetOfJobPosting()
        {
            ISearchResponse<JobPostingDTO> searchResponse = await elasticClient.SearchAsync<JobPostingDTO>(
                s => s.Size(12)
                .Sort(q => q.Descending(u => u.DateAdded)
                ));

            IReadOnlyCollection<JobPostingDTO> JobPosting = searchResponse.Documents;
            return (List<JobPostingDTO>)JobPosting;

        }

        public async Task CreateJobPostingBulk(List<JobPosting> jobPostings)
        {

        }


        public void UpdateJobPosting(JobPosting jobPosting)
        {


        }
    }
}
