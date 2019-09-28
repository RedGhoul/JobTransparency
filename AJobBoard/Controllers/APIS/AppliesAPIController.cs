using AJobBoard.Data;
using AJobBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppliesAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppliesAPIController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/AppliesAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Apply>>> GetApplies()
        {
            var User = await _userManager.GetUserAsync(HttpContext.User);

            var applications = await _context.Applies.Include(x => x.JobPosting)
                .Where(x => x.Applier.Id == User.Id).ToListAsync();

            var apps = applications.Select(x => new
            {
                Id = x.Id,
                JobId = x.JobPosting.Id,
                Title = x.JobPosting.Title,
                Company = x.JobPosting.Company,
                Location = x.JobPosting.Location,
                JobSource = x.JobPosting.JobSource,
                Applicates = x.JobPosting.NumberOfApplies,
                Views = x.JobPosting.NumberOfViews,
                URL = x.JobPosting.URL,
                PostDate = x.JobPosting.PostDate
            }).ToList();
            if (apps != null)
            {
                return Ok(new { data = apps });
            }
            return Ok(new { data = "" });
        }

        // GET: api/AppliesAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Apply>> GetApply(int id)
        {
            var apply = await _context.Applies.FindAsync(id);

            if (apply == null)
            {
                return NotFound();
            }

            return apply;
        }

        // PUT: api/AppliesAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApply(int id, Apply apply)
        {
            if (id != apply.Id)
            {
                return BadRequest();
            }

            _context.Entry(apply).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplyExists(id))
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

        // POST: api/AppliesAPI
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Apply>> PostApply(Apply apply)
        {

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return BadRequest("Please Sign in to Add to Applies");
            }
            // Have to seprate this stuff out in the future
            var job = _context.JobPostings.Where(x => x.Id == apply.Id).FirstOrDefault();
            job.NumberOfApplies++;

            try
            {
                _context.Update(job);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return NotFound(ex);
            }

            if (job != null)
            {
                if (currentUser.Applies != null)
                {
                    currentUser.Applies.Add(new Apply
                    {
                        DateAddedToApplies = DateTime.Now,
                        JobPosting = job
                    });
                }
                else
                {
                    currentUser.Applies = new List<Apply>();
                    currentUser.Applies.Add(new Apply
                    {
                        DateAddedToApplies = DateTime.Now,
                        JobPosting = job
                    });
                }

                await _context.SaveChangesAsync();
            }

            return Ok();

            //return CreatedAtAction("GetApply", new { id = apply.Id }, apply);
        }

        // DELETE: api/AppliesAPI/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Apply>> DeleteApply(int id)
        {
            var apply = await _context.Applies.FindAsync(id);
            if (apply == null)
            {
                return NotFound();
            }

            _context.Applies.Remove(apply);
            await _context.SaveChangesAsync();

            return apply;
        }

        private bool ApplyExists(int id)
        {
            return _context.Applies.Any(e => e.Id == id);
        }
    }
}
