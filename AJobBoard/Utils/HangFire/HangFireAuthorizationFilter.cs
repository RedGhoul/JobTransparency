﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hangfire.Dashboard;

namespace AJobBoard.Utils.HangFire
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var useRole = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            return useRole == "Admin";
        }
    }
}
