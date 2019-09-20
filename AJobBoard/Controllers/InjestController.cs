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
using System.Web;
using System.Text;

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
            string Synopsis = "";
            using (StreamReader sr = new StreamReader(@"C:\Users\Avaneesa.Basappa\source\repos\JobTransparency\AJobBoard\IndeedJobDump20092019114811.csv"))
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

                        byte[] byte16 = Encoding.Default.GetBytes(HttpUtility.HtmlAttributeEncode(list[0].Trim()));
                        string myTitle = Encoding.UTF8.GetString(byte16);
                        string Title = myTitle;

                        byte[] byte17 = Encoding.Default.GetBytes(HttpUtility.HtmlAttributeEncode(list[1].Trim()));
                        string myJobURL = Encoding.UTF8.GetString(byte17);
                        string JobURL = myJobURL;

                        byte[] byte18 = Encoding.Default.GetBytes(HttpUtility.HtmlAttributeEncode(list[2].Trim()));
                        string myPostingDate = Encoding.UTF8.GetString(byte18);
                        string PostingDate = myPostingDate;

                        byte[] byte19 = Encoding.Default.GetBytes(HttpUtility.HtmlAttributeEncode(list[3].Trim()));
                        string myLocation = Encoding.UTF8.GetString(byte19);
                        string Location = myLocation;

                        byte[] byte20 = Encoding.Default.GetBytes(HttpUtility.HtmlAttributeEncode(list[4].Trim()));
                        string myCompany = Encoding.UTF8.GetString(byte20);
                        string Company = myCompany;

                        byte[] bytes21 = Encoding.Default.GetBytes(HttpUtility.HtmlAttributeEncode(list[5].Trim()));
                        string mySalary = Encoding.UTF8.GetString(bytes21);
                        string Salary = mySalary;


                        byte[] bytes22 = Encoding.Default.GetBytes(HttpUtility.HtmlAttributeEncode(list[6].Trim()));
                        string mySynopsis = Encoding.UTF8.GetString(bytes22);
                        Synopsis = mySynopsis;

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
 
                       await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(Synopsis);
                        Console.WriteLine("ERROR");
                    }
                   
                   

                }
            }

            
            

            return Ok("Ingested Properly");
        }

    }
}
