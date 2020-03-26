using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Services;
using Microsoft.AspNetCore.Authorization;

namespace AJobBoard.Controllers.Views
{
    [Authorize(Roles = "Admin")]
    public class ETLStatusController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly INLTKService _NLTKService;
        private readonly IKeyPharseRepository _KeyPharseRepository;
        private readonly ElasticService _es;
        private readonly ApplicationDbContext _ctx;
        public ETLStatusController(IJobPostingRepository jobPostingRepository,
            INLTKService NLTKService,
            IKeyPharseRepository KeyPharseRepository,
            ElasticService es,
            ApplicationDbContext ctx)
        {
            _jobPostingRepository = jobPostingRepository;
            _NLTKService = NLTKService;
            _KeyPharseRepository = KeyPharseRepository;
            _es = es;
            _ctx = ctx;
        }

        // GET: ETLStatus
        public async Task<IActionResult> Index()
        {
            return View(await _ctx.ETLStatus.ToListAsync());
        }

        // GET: ETLStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eTLStatus = await _ctx.ETLStatus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eTLStatus == null)
            {
                return NotFound();
            }

            return View(eTLStatus);
        }

        // GET: ETLStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ETLStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ETLStatus eTLStatus)
        {
            if (!string.IsNullOrEmpty(eTLStatus.Name))
            {
                eTLStatus.Started = DateTime.Now;
                _ctx.Add(eTLStatus);
                await _ctx.SaveChangesAsync();
                var status = await _ctx.ETLStatus.OrderByDescending(x => x.Started)
                .Where(x => x.Finished == false)
                .FirstOrDefaultAsync();

                if (status != null)
                {
                    IEnumerable<JobPosting> things = await _jobPostingRepository.GetJobPostingsWithKeyPhraseAsync(10000);

                    foreach (var JobPosting in things)
                    {
                        bool change = false;

                        if (JobPosting.KeyPhrases == null || JobPosting.KeyPhrases.Count == 0)
                        {
                            var wrapper = await _NLTKService.GetNLTKKeyPhrases(JobPosting.Description);
                            if (wrapper != null && wrapper.rank_list != null && wrapper.rank_list.Count > 0)
                            {
                                var ListKeyPhrase = new List<KeyPhrase>();

                                foreach (var item in wrapper.rank_list)
                                {
                                    if (double.Parse(item.Affinty) > 20)
                                    {
                                        ListKeyPhrase.Add(new KeyPhrase
                                        {
                                            Affinty = item.Affinty,
                                            Text = item.Text,
                                            JobPosting = JobPosting
                                        });
                                    }

                                }

                                await _KeyPharseRepository.CreateKeyPhrasesAsync(ListKeyPhrase);

                                JobPosting.KeyPhrases = ListKeyPhrase;
                                change = true;
                            }


                        }

                        if (string.IsNullOrEmpty(JobPosting.Summary))
                        {
                            var NLTKSummary = await _NLTKService.GetNLTKSummary(JobPosting.Description);

                            JobPosting.Summary = NLTKSummary.SummaryText;
                            change = true;
                        }

                        if (change == true)
                        {
                            await _jobPostingRepository.PutJobPostingAsync(JobPosting.Id, JobPosting);
                            change = false;
                        }
                    }

                    status.Finished = true;
                    status.Ended = DateTime.Now;
                    await _ctx.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(eTLStatus);

            }
            return View(eTLStatus);
        }

        // GET: ETLStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eTLStatus = await _ctx.ETLStatus.FindAsync(id);
            if (eTLStatus == null)
            {
                return NotFound();
            }
            return View(eTLStatus);
        }

        // POST: ETLStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Finished,Started,Ended")] ETLStatus eTLStatus)
        {
            if (id != eTLStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _ctx.Update(eTLStatus);
                    await _ctx.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ETLStatusExists(eTLStatus.Id))
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
            return View(eTLStatus);
        }

        // GET: ETLStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eTLStatus = await _ctx.ETLStatus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eTLStatus == null)
            {
                return NotFound();
            }

            return View(eTLStatus);
        }

        // POST: ETLStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eTLStatus = await _ctx.ETLStatus.FindAsync(id);
            _ctx.ETLStatus.Remove(eTLStatus);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ETLStatusExists(int id)
        {
            return _ctx.ETLStatus.Any(e => e.Id == id);
        }


    }
}
