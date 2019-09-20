using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AJobBoard.Data;
using AJobBoard.Models;
using System.IO;

namespace AJobBoard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InjestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InjestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/InjestJobPostings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetInjests()
        {
            using (StreamReader sr = new StreamReader(@"C:\Users\Avane\source\repos\AJobBoard\AJobBoard\IndeedJobDump19092019200410.csv"))
            {
                int count = 0;
                String line;
                while ((line = sr.ReadLine()) != null)
                {

                    try
                    {
                        if (line.Contains("Synopsis"))
                        {
                            continue;
                        }
                        string[] list = line.Split(",");
                        string Title = list[0].Trim();
                        string JobURL = list[1].Trim();
                        string PostingDate = list[2].Trim();
                        string Location = list[3].Trim();
                        string Company = list[4].Trim();
                        string Salary = list[5].Trim();
                        string Synopsis = list[6].Trim();

                        var JobPosting = new JobPosting()
                        {
                            Title = Title,
                            URL = JobURL,
                            PostDate = PostingDate,
                            Location = Location,
                            Company = Company,
                            Salary = Salary,
                            Summary = Synopsis,
                            JobSource = "Indeed"
                        };
                        _context.JobPostings.Add(JobPosting);
                        count++;
                        if (count % 20 == 0)
                        {
                            await _context.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR");
                    }
                   
                   

                }
            }

            
            

            return Ok("Ingested Properly");
        }

    }
}
