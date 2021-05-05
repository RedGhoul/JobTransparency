using AJobBoard.Data;
using AJobBoard.Utils.HangFire;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobtransparency.HangFire
{
    public class GenerateKeyTopKeyWordsJobs 
        //: ICustomJob
    {
        //private readonly ApplicationDbContext _ctx;

        //public GenerateKeyTopKeyWordsJobs(ApplicationDbContext ctx)
        //{
        //    _ctx = ctx;
        //}

        //public async Task Run(IJobCancellationToken token)
        //{
        //    token.ThrowIfCancellationRequested();
        //    await RunAtTimeOf(DateTime.Now);
        //}

        //public Task RunAtTimeOf(DateTime now)
        //{
        //    var allJobs = _ctx.JobPostings.ToList();
        //    //foreach (var item in allJobs)
        //    //{
        //    //    _ctx.KeyPhrase.Where(x => x.JobPostingId == item.Id && x.Affinty > ).Select(x => x.)
        //    //}
            
        //}
    }
}
