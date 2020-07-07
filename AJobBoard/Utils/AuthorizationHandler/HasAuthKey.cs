using AJobBoard.Utils.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AJobBoard.Utils.AuthorizationHandler
{
    public class HasAuthKey : AuthorizationHandler<HasAuthKey>, IAuthorizationRequirement
    {
        IConfiguration _configuration;

        public HasAuthKey(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasAuthKey requirment)
        {
            var authContext = (AuthorizationFilterContext)context.Resource;
            if (authContext.HttpContext.Request.Headers.ContainsKey("Auth") == true)
            {
                string Key = authContext.HttpContext.Request.Headers["Auth"];
                if (Key.Equals(Secrets.getAppSettingsValue(_configuration, "Auth-AzureFunction"))) ;
                {
                    context.Succeed(requirment);
                }
            }

            return Task.CompletedTask;
        }
    }
}
