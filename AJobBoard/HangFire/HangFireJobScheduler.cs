﻿using AJobBoard.Utils.HangFire;
using Hangfire;
using Jobtransparency.HangFire;
using System;

namespace Jobtransparency.Utils.HangFire
{
    public static class HangFireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            RecurringJob.RemoveIfExists(nameof(RemotiveIoDataJob));
            RecurringJob.AddOrUpdate<RemotiveIoDataJob>(nameof(RemotiveIoDataJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(1, 12), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists(nameof(GenerateKeyTopKeyWordsJobs));
            RecurringJob.AddOrUpdate<GenerateKeyTopKeyWordsJobs>(nameof(GenerateKeyTopKeyWordsJobs),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(1, 20), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists(nameof(OkRemoteJobByTags));
            RecurringJob.AddOrUpdate<OkRemoteJobByTags>(nameof(OkRemoteJobByTags),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(1, 10), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists(nameof(OkRemoteJob));
            RecurringJob.AddOrUpdate<OkRemoteJob>(nameof(OkRemoteJob),
                job => job.Run(JobCancellationToken.Null),
                "0 */2 * * *", TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists(nameof(GetJobPostingsJob));
            RecurringJob.AddOrUpdate<GetJobPostingsJob>(nameof(GetJobPostingsJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(1, 20), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists(nameof(KeyPhraseGeneratorJob));
            RecurringJob.AddOrUpdate<KeyPhraseGeneratorJob>(nameof(KeyPhraseGeneratorJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(2, 20), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists(nameof(SummaryGeneratorJob));
            RecurringJob.AddOrUpdate<SummaryGeneratorJob>(nameof(SummaryGeneratorJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(3, 20), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists(nameof(SentimentGeneratorJob));
            RecurringJob.AddOrUpdate<SentimentGeneratorJob>(nameof(SentimentGeneratorJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(4, 20), TimeZoneInfo.Utc);

            RecurringJob.RemoveIfExists(nameof(IsJobExpiredJobPostingsJob));
            RecurringJob.AddOrUpdate<IsJobExpiredJobPostingsJob>(nameof(IsJobExpiredJobPostingsJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Weekly(DayOfWeek.Monday, 1, 1), TimeZoneInfo.Utc);
        }
    }
}
