using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AJobBoard.Data;
using AJobBoard.Models;
using System.IO;
using System.Text;
using System.Web;

namespace AJobBoard.Controllers
{
    public class JobPostingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobPostingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string Location, string KeyWords, int? MaxResults)
        {
            ViewBag.TotalJobs = await _context.JobPostings.CountAsync();
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            List<JobPosting> Jobs = null;
            if (MaxResults == 0 || Location == null)
            {
                Jobs = await _context.JobPostings.OrderByDescending(j => j.Title)
                    .Where(x => x.Location.Contains(Location ?? "") &&
                           x.Summary.Contains(KeyWords ?? "") &&
                           x.Title.Contains(KeyWords ?? ""))
                    .Take(50)
                    .Distinct()
                    .ToListAsync();
                start = DateTime.Now;
            }
            else
            {
                if (Location != null && Location.ToLower().Equals("anywhere"))
                {
                    Jobs = await _context.JobPostings.OrderByDescending(j => j.Title)
                        .Where(x => x.Summary.Contains(KeyWords ?? "") &&
                               x.Title.Contains(KeyWords ?? ""))
                        .Take((int)MaxResults)
                        .Distinct()
                        .ToListAsync();
                    start = DateTime.Now;
                }
                else
                {
                    Jobs = await _context.JobPostings.OrderByDescending(j => j.Title)
                        .Where(x => x.Location.Contains(Location ?? "") &&
                               x.Summary.Contains(KeyWords ?? "") &&
                               x.Title.Contains(KeyWords ?? ""))
                        .Take((int)MaxResults)
                        .Distinct()
                        .ToListAsync();
                    start = DateTime.Now;
                }

            }
            TimeSpan duration = end - start;
            ViewBag.SecsToQuery = duration.TotalSeconds.ToString().Replace("-","");
            return View(Jobs);
        }

        [HttpPost]
        public IActionResult Find(HomeIndexViewModel homeIndexVM)
        {
            return RedirectToAction("Index", new
            {
                Location = homeIndexVM.FindModel.Location,
                KeyWords = homeIndexVM.FindModel.KeyWords,
                MaxResults = homeIndexVM.FindModel.MaxResults
            });
        }

        // GET: JobPostings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            return View(jobPosting);
        }

        // GET: JobPostings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobPostings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Summary,URL,Company,Location,PostDate,Salary,Posters")] JobPosting jobPosting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobPosting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobPosting);
        }

        // GET: JobPostings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting == null)
            {
                return NotFound();
            }
            return View(jobPosting);
        }

        // POST: JobPostings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Summary,URL,Company,Location,PostDate,Salary,Posters")] JobPosting jobPosting)
        {
            if (id != jobPosting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobPosting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobPostingExists(jobPosting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(jobPosting);
        }

        // GET: JobPostings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            return View(jobPosting);
        }

        // POST: JobPostings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            _context.JobPostings.Remove(jobPosting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobPostingExists(int id)
        {
            return _context.JobPostings.Any(e => e.Id == id);
        }

        public async Task<IActionResult> INTODB()
        {
            await DataIngesterAsync(_context);
            return RedirectToAction(nameof(Index));
        }

        public async Task DataIngesterAsync(ApplicationDbContext context)
        {
            string Synopsis = "";
            using (StreamReader sr = new StreamReader(@"C:\Users\Avane\source\repos\AJobBoard\AJobBoard\IndeedJobDump23092019212602NEW.csv"))
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
                        context.JobPostings.Add(JobPosting);
                        count++;

                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(Synopsis);
                        Console.WriteLine("ERROR");
                    }



                }
            }
        }
    }
}
