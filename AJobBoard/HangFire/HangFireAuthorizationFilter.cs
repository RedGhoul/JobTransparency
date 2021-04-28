using Hangfire.Dashboard;
using System.Security.Claims;

namespace AJobBoard.Utils.HangFire
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            Microsoft.AspNetCore.Http.HttpContext httpContext = context.GetHttpContext();
            if (httpContext == null) return false;
            string useRole = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (useRole == null) return false;
            return useRole.Equals("Admin");
        }
    }
}
