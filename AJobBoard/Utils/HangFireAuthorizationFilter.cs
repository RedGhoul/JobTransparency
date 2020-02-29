﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Dashboard;

namespace AJobBoard.Utils
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            //// Allow all authenticated users to see the Dashboard (potentially dangerous).
            //return httpContext.User.Identity.IsAuthenticated;
            bool boolAuthorizeCurrentUserToAccessHangFireDashboard = false;

            if (httpContext.User.Identity.IsAuthenticated)
            {
                if (httpContext.User.IsInRole("Admin"))
                    boolAuthorizeCurrentUserToAccessHangFireDashboard = true;
            }

            return boolAuthorizeCurrentUserToAccessHangFireDashboard;
        }
    }
}