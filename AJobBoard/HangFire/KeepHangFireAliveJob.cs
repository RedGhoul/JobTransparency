using Hangfire;
using Microsoft.Extensions.Logging;
using System;

namespace Jobtransparency.HangFire
{
    public class KeepHangFireAliveJob
    {
        private readonly ILogger<KeepHangFireAliveJob> _logger;

        public KeepHangFireAliveJob(ILogger<KeepHangFireAliveJob> logger)
        {
            _logger = logger;
        }


        public void Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            RunAtTimeOf(DateTime.Now);
        }

        public void RunAtTimeOf(DateTime now)
        {
            _logger.LogInformation("KeepHangFireAliveJob Starts... ");

            _logger.LogInformation("KeepHangFireAliveJob Ends... ");
        }
    }
}
