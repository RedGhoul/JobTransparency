using AJobBoard.Utils.HangFire;
using Hangfire;
using System;

namespace Jobtransparency.Utils.HangFire
{
    public static class HangFireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            RecurringJob.RemoveIfExists(nameof(KeyPhraseGeneratorJob));
            RecurringJob.AddOrUpdate<KeyPhraseGeneratorJob>(nameof(KeyPhraseGeneratorJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(12, 20), TimeZoneInfo.Local);

            RecurringJob.RemoveIfExists(nameof(SummaryGeneratorJob));
            RecurringJob.AddOrUpdate<SummaryGeneratorJob>(nameof(SummaryGeneratorJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(1, 20), TimeZoneInfo.Local);

            RecurringJob.RemoveIfExists(nameof(GetJobPostingsJob));
            RecurringJob.AddOrUpdate<GetJobPostingsJob>(nameof(GetJobPostingsJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(3, 20), TimeZoneInfo.Local);

            RecurringJob.RemoveIfExists(nameof(IsJobExpiredJobPostingsJob));
            RecurringJob.AddOrUpdate<IsJobExpiredJobPostingsJob>(nameof(IsJobExpiredJobPostingsJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(5, 20), TimeZoneInfo.Local);
        }
    }
}
