using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;

namespace AJobBoard.Utils
{
    public class HangFireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            RecurringJob.RemoveIfExists(nameof(MyJob));
            RecurringJob.AddOrUpdate<MyJob>(nameof(MyJob),
                job => job.Run(JobCancellationToken.Null),
                Cron.Daily(5,00),TimeZoneInfo.Local);
        }
    }
}
