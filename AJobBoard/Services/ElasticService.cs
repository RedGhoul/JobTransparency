﻿using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AJobBoard.Utils.Config;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .Source(sf => sf.Excludes(e => e.Fields("key*")))
                .Query(q => q
                     .Match(m => m
                        .Field(f => f.Location)
                        .Query(keywords)
                     ))
                .Query(q => q
                     .Match(m => m
                        .Field(f => f.Company)
                        .Query(keywords)
                     ))
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

        public List<JobPostingDTO> GetAllJobPostings()
        {
            List<JobPostingDTO> indexedList = new List<JobPostingDTO>();
            var scanResults = elasticClient.Search<JobPostingDTO>(s => s
                            .From(0)
                            .Size(5)
                            .MatchAll()
                            //I used field to get only the value I needed rather than getting the whole document

                            .Scroll("5m")
                        );

            var results = elasticClient.Scroll<JobPostingDTO>("10m", scanResults.ScrollId);
            while (results.Documents.Any())
            {
                foreach (var doc in results.Documents)
                {
                    indexedList.Add(doc);
                }

                results = elasticClient.Scroll<JobPostingDTO>("10m", results.ScrollId);
            }
            return indexedList;
        }


        public async Task DeleteJobPostingIndexAsync()
        {
            await elasticClient.Indices.DeleteAsync("jobpostings");
        }

        public async Task CreateJobPostingAsync(JobPostingDTO jobPosting)
        {
            await elasticClient.IndexDocumentAsync(jobPosting);
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
