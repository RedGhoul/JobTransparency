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
                Cron.Weekly(DayOfWeek.Monday, 5, 33), TimeZoneInfo.Local, "keyphrase_generator_job");

            RecurringJob.RemoveIfExists(nameof(SummaryGeneratorJob));
            RecurringJob.AddOrUpdate<SummaryGeneratorJob>(nameof(SummaryGeneratorJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Weekly(DayOfWeek.Friday, 5, 33), TimeZoneInfo.Local, "summary_generator_job");

            RecurringJob.RemoveIfExists(nameof(ReIndexJobPostingsJob));
            RecurringJob.AddOrUpdate<ReIndexJobPostingsJob>(nameof(ReIndexJobPostingsJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Monthly(27, 1), TimeZoneInfo.Local, "reIndex_jobpostings_job");
        }
    }
}
