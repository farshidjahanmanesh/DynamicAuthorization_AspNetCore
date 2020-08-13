using IdentityLearning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Reflection;
using IdentityLearning.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace IdentityLearning.ViewComponents
{
    public class NavBarViewComponent : ViewComponent
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public NavBarViewComponent(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {



            var accessLevel = new List<NavBarViewModel>();
            if (!User.Identity.IsAuthenticated)
            {
                return View(accessLevel);
            }
            var claimsList = HttpContext.User.Claims;
            var assembly = Assembly.GetExecutingAssembly();
            var controllers = assembly.GetTypes().Where(x => x.Name.EndsWith("Controller"));


           


            foreach (var item in controllers)
            {
                bool checkControllerPolicy = true;
                string persianControllerAccessname = "";
                if (item.CustomAttributes.Any(c => c.AttributeType == typeof(AuthorizeAttribute)))
                {
                    bool controllerMarkToShow = false;

                    object[] policyAtts = item.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                    if (item.CustomAttributes.Any(c => c.AttributeType == typeof(MarkedToNavBarAttribute)))
                    {
                        persianControllerAccessname = item.GetCustomAttribute<MarkedToNavBarAttribute>().name;
                        controllerMarkToShow = true;
                    }

                    foreach (AuthorizeAttribute attItem in policyAtts)
                    {
                        var policyName = attItem.Policy;
                        if (!claimsList.Any(c => c.Type == "AccessLevel" && c.Value == policyName))
                        {
                            checkControllerPolicy = false;
                        }
                        else
                        {
                            if (controllerMarkToShow)
                            {
                                // persianControllerAccessname = permissionsPersianNames.First(x => x.Key == policyName).Value;
                                accessLevel.Add(new NavBarViewModel()
                                {
                                    ActionName = "",
                                    ControllerName = item.Name.Remove(item.Name.Length - 10),
                                    PersianAccessLevelName = persianControllerAccessname
                                });
                            }

                        }
                    }

                }
                if (checkControllerPolicy)
                {

                    var methodInfos = item.GetMethods().Where(x => x.CustomAttributes
                                                                .Any(c => c.AttributeType == typeof(MarkedToNavBarAttribute)));
                    foreach (var methodItem in methodInfos)
                    {
                        bool methodMarkToShow = false;
                        string PersianMethodAccessName = "";
                        if (methodItem.CustomAttributes.Any(x => x.AttributeType == typeof(MarkedToNavBarAttribute)))
                        {
                            PersianMethodAccessName = methodItem.GetCustomAttribute<MarkedToNavBarAttribute>().name;
                            methodMarkToShow = true;
                        }
                        if (methodMarkToShow)
                        {
                            object[] methodAtts = methodItem.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                            if (methodAtts == null || methodAtts.Length == 0)
                            {
                                accessLevel.Add(new NavBarViewModel()
                                {
                                    ActionName = methodItem.Name,
                                    ControllerName = item.Name.Remove(item.Name.Length - 10),
                                    PersianAccessLevelName = persianControllerAccessname

                                });
                            }
                            if (methodAtts.Count() == 1 && methodAtts[0] is AuthorizeAttribute)
                            {
                                var justAuthorizeAtt = methodAtts[0] as AuthorizeAttribute;

                                if (justAuthorizeAtt.Policy == "")
                                {
                                    accessLevel.Add(new NavBarViewModel()
                                    {
                                        ActionName = methodItem.Name,
                                        ControllerName = item.Name.Remove(item.Name.Length - 10),
                                        PersianAccessLevelName = PersianMethodAccessName

                                    });
                                }
                            }
                            foreach (AuthorizeAttribute att in methodAtts)
                            {
                                var policyName = att.Policy;
                                if (claimsList.Any(c => c.Type == "AccessLevel" && c.Value == policyName))
                                {

                                    if (!accessLevel.Any(x => x.ActionName == methodItem.Name
                                                            && x.PersianAccessLevelName == PersianMethodAccessName))
                                    {
                                        accessLevel.Add(new NavBarViewModel()
                                        {
                                            ActionName = methodItem.Name,
                                            ControllerName = item.Name.Remove(item.Name.Length - 10),
                                            PersianAccessLevelName = PersianMethodAccessName
                                        });
                                    }

                                }
                            }
                        }

                    }
                }
            }
            return View(accessLevel);
        }
    }
}
