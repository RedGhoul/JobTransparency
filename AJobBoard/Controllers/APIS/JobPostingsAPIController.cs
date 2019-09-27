using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AJobBoard.Data;
using AJobBoard.Models;

namespace AJobBoard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostingsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JobPostingsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/JobPostingsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetJobPostings()
        {
            return await _context.JobPostings.ToListAsync();
        }

        // GET: api/JobPostingsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobPosting>> GetJobPosting(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            return jobPosting;
        }

        // PUT: api/JobPostingsAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobPosting(int id, JobPosting jobPosting)
        {
            if (id != jobPosting.Id)
            {
                return BadRequest();
            }

            _context.Entry(jobPosting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobPostingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/JobPostingsAPI
        [HttpPost]
        public async Task<ActionResult<JobPosting>> PostJobPosting(JobPosting jobPosting)
        {
            _context.JobPostings.Add(jobPosting);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJobPosting", new { id = jobPosting.Id }, jobPosting);
        }

        // DELETE: api/JobPostingsAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<JobPosting>> DeleteJobPosting(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            _context.JobPostings.Remove(jobPosting);
            await _context.SaveChangesAsync();

            return jobPosting;
        }

        private bool JobPostingExists(int id)
        {
            return _context.JobPostings.Any(e => e.Id == id);
        }
    }
}
