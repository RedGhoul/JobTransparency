using System;
using AJobBoard.Utils.HangFire;
using Hangfire;

namespace Jobtransparency.Utils.HangFire
{
    public static class HangFireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            RecurringJob.RemoveIfExists(nameof(KeyPhraseGeneratorJob));
            RecurringJob.AddOrUpdate<KeyPhraseGeneratorJob>(nameof(KeyPhraseGeneratorJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(5, 30), TimeZoneInfo.Local);

            RecurringJob.RemoveIfExists(nameof(SummaryGeneratorJob));
            RecurringJob.AddOrUpdate<SummaryGeneratorJob>(nameof(SummaryGeneratorJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(10,10), TimeZoneInfo.Local);
        }
    }
}
