using AJobBoard.Data;
using AJobBoard.Models.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RandomUserAgent;
using RestSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AJobBoard.Utils.Seeder
{
    public static class Seeder
    {

        public static async Task CreateUserRoles(IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                RoleManager<IdentityRole> RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                UserManager<ApplicationUser> UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                //var JobsRepo = scope.ServiceProvider.GetRequiredService<IJobPostingRepository>();
                //await JobsRepo.BuildCache();
                ApplicationDbContext content = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                IdentityResult roleResult;

                //Adding Admin Role
                bool roleCheck = await RoleManager.RoleExistsAsync("Admin");
                if (!roleCheck)
                {
                    //create the roles and seed them to the database
                    roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
                }

                //Assign Admin role to the main User here we have given our newly registered 
                //login id for Admin management
                // Also Assigning them Claims to perform CUD operations
                ApplicationUser user = await UserManager.FindByEmailAsync("avaneesab5@gmail.com");
                if (user != null)
                {
                    System.Collections.Generic.IList<string> currentUserRoles = await UserManager.GetRolesAsync(user);
                    if (!currentUserRoles.Contains("Admin"))
                    {
                        await UserManager.AddToRoleAsync(user, "Admin");
                    }

                    System.Collections.Generic.IList<Claim> currentClaims = await UserManager.GetClaimsAsync(user);
                    if (!currentClaims.Any())
                    {
                        Claim CanCreatePostingClaim = new Claim("CanCreatePosting", "True");
                        await UserManager.AddClaimAsync(user, CanCreatePostingClaim);

                        Claim CanEditPostingClaim = new Claim("CanEditPosting", "True");
                        await UserManager.AddClaimAsync(user, CanEditPostingClaim);

                        Claim CanDeletePostingClaim = new Claim("CanDeletePosting", "True");
                        await UserManager.AddClaimAsync(user, CanDeletePostingClaim);
                    }
                }
            
            }

        }
    }
}