using IdentityLearning.Models.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityLearning.Models
{
    //public class PolicyAttribute : AuthorizeAttribute
    //{
    //    public PolicyAttribute(params persmission[] persmissions)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        foreach (var item in persmissions)
    //        {
    //            sb.Append(item);
    //        }
    //        Policy = sb.ToString();
    //    }

    //}
    public class PolicyAuthorize : IAuthorizationService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public PolicyAuthorize(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            //for when not requirement use
            if (requirements.Count() == 1
                && requirements.First() is DenyAnonymousAuthorizationRequirement)
                return await Task.FromResult<AuthorizationResult>(AuthorizationResult.Success());

            //for when user not login 
            if (user == null || user.Identity.IsAuthenticated == false)
                return await Task.FromResult<AuthorizationResult>(AuthorizationResult.Failed());

           // var UserAccount = await userManager.FindByNameAsync(user.Identity.Name);
           // var UserRolenames = await userManager.GetRolesAsync(UserAccount);
            bool result = true;
            var claimsList = user.Claims;
            if (requirements.Count() > 0)
                if (claimsList.Count() == 0)
                    return await Task.FromResult<AuthorizationResult>(AuthorizationResult.Failed());

            foreach(var item in requirements)
            {
               // ClaimsAuthorizationRequirement temp;
                if(item is ClaimsAuthorizationRequirement temp)
                {
                    foreach(var SubItem in temp.AllowedValues)
                    {
                        if(!claimsList.Any(c=>c.Type==temp.ClaimType
                        &&c.Value==SubItem))
                        {
                            result = false;
                            break;
                        }
                    }
                }

                if (result == false)
                    break;
            }








            if (result)
                return await Task.FromResult<AuthorizationResult>(AuthorizationResult.Success());
            else
                return await Task.FromResult<AuthorizationResult>(AuthorizationResult.Failed());

        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
        {
            throw new NotImplementedException();
        }
    }
    public class User : IdentityUser
    {
        public string AboutMe { get; set; }
        public string PersianName { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsActive { get; set; }
        public bool IsExternalLogin { get; set; }

    }
    public class Role : IdentityRole
    {

    }



    public class TestIdentityDbContext : IdentityDbContext<User>
    {
        public DbSet<ViewerCounter> ViewerCounter { get; set; }
        public DbSet<DeviceCounter> DeviceCounter { get; set; }
        public DbSet<BrowserCounter> BrowserCounter { get; set; }
        public TestIdentityDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //this.user
            base.OnModelCreating(builder);
        }

    }
}
