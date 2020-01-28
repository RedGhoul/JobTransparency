using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AJobBoard.Utils
{
    public class HangfireDashboardMiddleware
    {
        private readonly DashboardOptions _dashboardOptions;
        private readonly JobStorage _jobStorage;
        private readonly RequestDelegate _nextRequestDelegate;
        private readonly RouteCollection _routeCollection;

        public HangfireDashboardMiddleware(
            RequestDelegate nextRequestDelegate,
            JobStorage storage,
            DashboardOptions options,
            RouteCollection routes)
        {
            _nextRequestDelegate = nextRequestDelegate;
            _jobStorage = storage;
            _dashboardOptions = options;
            _routeCollection = routes;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var aspNetCoreDashboardContext =
                new AspNetCoreDashboardContext(_jobStorage, _dashboardOptions, httpContext);

            var findResult = _routeCollection.FindDispatcher(httpContext.Request.Path.Value);
            if (findResult == null)
            {
                await _nextRequestDelegate.Invoke(httpContext);
                return;
            }

            // attempt to authenticate against default auth scheme (this will attempt to authenticate using data in request, but doesn't send challenge)
            var result = await httpContext.AuthenticateAsync();

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                // request was not authenticated, send challenge and do not continue processing this request
                await httpContext.ChallengeAsync();
            }


            var isAuthenticated = httpContext.User?.IsInRole("Admin");
            httpContext.Response.StatusCode = isAuthenticated == true
                ? (int)HttpStatusCode.Accepted
                : (int)HttpStatusCode.Unauthorized;

            if (isAuthenticated == false)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                return;
            }

            if (_dashboardOptions
                .Authorization
                .Any(filter =>
                         filter.Authorize(aspNetCoreDashboardContext) == false))
            {
                var isAuthenticatedValue = httpContext.User?.Identity?.IsAuthenticated;
                httpContext.Response.StatusCode = isAuthenticatedValue == true
                                                      ? (int)HttpStatusCode.Forbidden
                                                      : (int)HttpStatusCode.Unauthorized;
                return;
            }

            aspNetCoreDashboardContext.UriMatch = findResult.Item2;
            await findResult.Item1.Dispatch(aspNetCoreDashboardContext);
        }
    }
}
