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
            using (StreamReader sr = new StreamReader(@"C:\Users\Avane\source\repos\AJobBoard\AJobBoard\IndeedJobDump08092019195015.csv"))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {


                    if (line.Contains("Synopsis"))
                    {
                        continue;
                    }
                    string[] list = line.Split(",");
                    string Title = list[0];
                    string JobURL = list[1];
                    string PostingDate = list[2];
                    string Location = list[3];
                    string Company = list[4];
                    string Salary = list[5];
                    string Synopsis = list[6];

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
                    
                }
            }

            await _context.SaveChangesAsync();

            return await _context.JobPostings.ToListAsync();
        }

    }
}
