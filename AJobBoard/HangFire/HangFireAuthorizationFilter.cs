using Hangfire.Dashboard;
using System.Security.Claims;

namespace AJobBoard.Utils.HangFire
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            Microsoft.AspNetCore.Http.HttpContext httpContext = context.GetHttpContext();
            string useRole = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            return useRole == "Admin";
        }
    }
}
