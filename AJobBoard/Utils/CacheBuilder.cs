using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AJobBoard.Data;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace AJobBoard.Utils
{
    public class CacheBuilder
    {
        IServiceProvider _serviceProvider;
        public CacheBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Build()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var JobsRepo = scope.ServiceProvider.GetRequiredService<IJobPostingRepository>();
                await JobsRepo.BuildCache();
            }
        }
    }

}
