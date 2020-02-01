using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AJobBoard.Models;
using AJobBoard.Models.DTO;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;

namespace AJobBoard.Services
{
    public class ElasticService
    {


        public ElasticClient SetUp()
        {
            var node = new Uri("http://a-main-elastic.experimentsinthedeep.com");
            var settings = new ConnectionSettings(node).DefaultIndex("jobpostings");
            var client = new ElasticClient(settings);
            return client;
        }
        public async Task<List<JobPosting>> QueryJobPosting(int fromNumber, string keywords)
        {
            var client = new HttpClient();
            //http://a-main-elastic.experimentsinthedeep.com/jobpostings/_search?q=Description:aws&from=12&size=12
            HttpResponseMessage response = await client.GetAsync("http://a-main-elastic.experimentsinthedeep.com/jobpostings/_search?q=Description:" + keywords + "&from=" + fromNumber + "&size="+12);

            var result = response.Content.ReadAsStringAsync().Result;

            try
            {
                RootObject list = JsonConvert
                    .DeserializeObject<RootObject>(result);
                List<JobPosting> listsJobPostings = new List<JobPosting>();
                foreach (var item in list.hits.hits)
                {
                    
                    listsJobPostings.Add(item._source);
                }
                return listsJobPostings;
            }
            catch (Exception ex)
            {
                return null;
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

            var response = await client.PutAsync("http://a-main-elastic.experimentsinthedeep.com" + "/jobpostings/_doc/" + jobPosting.Id, data);
        }

        public async Task CreateJobPostingBulk(List<JobPosting> jobPostings)
        {
            var indexManyAsyncResponse = await SetUp().IndexManyAsync(jobPostings);
            if (indexManyAsyncResponse.Errors)
            {
                foreach (var itemWithError in indexManyAsyncResponse.ItemsWithErrors)
                {
                    Console.WriteLine("Failed to index document {0}: {1}", itemWithError.Id, itemWithError.Error);
                }
            }
        }


        public void UpdateJobPosting(JobPosting jobPosting)
        {
            try
            { 
                var job = SetUp().Update<JobPosting,object>(jobPosting.Id.ToString(), 
                    u => u.Doc(jobPosting));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }
        public async Task<DeleteResponse> DeleteJobPosting(JobPosting jobPosting)
        {
            try
            {
                var getResponse = SetUp().Get<JobPosting>(jobPosting.Id.ToString());

                var job = getResponse.Source;

                var updateResponse = await SetUp().DeleteAsync<JobPosting>(job);
                return updateResponse;
                //var job = await _client.UpdateAsync<JobPosting>(jobPosting.Id,
                //    u => u.Index("jobposting").Doc(jobPosting));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

        public async Task<List<JobPosting>> GetRandomSetOfJobPosting()
        {
            var client = new HttpClient();
           
            var response = await client.GetAsync("http://a-main-elastic.experimentsinthedeep.com/jobpostings/_search?"+"&from=" + new Random().Next(1,12) + "&size=" + 12);

            var result = response.Content.ReadAsStringAsync().Result;

            try
            {
                RootObject list = JsonConvert
                    .DeserializeObject<RootObject>(result);
                List<JobPosting> listsJobPostings = new List<JobPosting>();
                foreach (var item in list.hits.hits)
                {

                    listsJobPostings.Add(item._source);
                }
                return listsJobPostings;
            }
            catch (Exception ex)
            {
                return null;
            }
         
            return null;
        }
    }
}
