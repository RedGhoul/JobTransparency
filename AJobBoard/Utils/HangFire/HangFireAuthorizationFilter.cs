using Hangfire.Dashboard;
using System.Security.Claims;

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
