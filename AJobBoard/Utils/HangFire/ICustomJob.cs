using Hangfire;
using System;
using System.Threading.Tasks;

namespace AJobBoard.Utils.HangFire
{
    public interface ICustomJob
    {
        public Task RunAtTimeOf(DateTime now);
        public Task Run(IJobCancellationToken token);
    }
}
