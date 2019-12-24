using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AJobBoard.Utils
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
            if(authContext.HttpContext.Request.Headers.ContainsKey("Auth") == true)
            {
                string Key = authContext.HttpContext.Request.Headers["Auth"];
                if (Key.Equals(_configuration.GetSection("AppSettings")["Auth-AzureFunction"]))
                {
                    context.Succeed(requirment);
                }
            }

            return Task.CompletedTask;
        }
    }
}
