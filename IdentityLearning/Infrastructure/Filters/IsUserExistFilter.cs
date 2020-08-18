using IdentityLearning.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedServices.Models.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filters
{

 

    public class IsUserExistFilter:ActionFilterAttribute
    {
        private readonly UserManager<ApplicationUser> userManager;

        public IsUserExistFilter(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context.ModelState.IsValid)
            {
                var args = context.ActionArguments;
                if (args.ContainsKey("id"))
                {
                    string Id = args["id"] as string;
                    if (!userManager.Users.Any(x => x.Id == Id))
                    {
                        context.ModelState.AddModelError("UserNotFound"
                            , "با این ای دی  یوزری در سیستم موجود نیست");
                    }
                }
            }
           
            

        }
    }
}
