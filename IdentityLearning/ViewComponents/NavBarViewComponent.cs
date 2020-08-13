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
    public class TopMenuViewComponent : ViewComponent
    {
        public TopMenuViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var accessLevel = new HashSet<NavBarViewModel>();
            if (!User.Identity.IsAuthenticated)
            {
                return View(new List<IGrouping<string, NavBarViewModel>>());
            }
            var claimsList = HttpContext.User.Claims;
            var assembly = Assembly.GetExecutingAssembly();
            var controllers = assembly.GetTypes().Where(x => x.Name.EndsWith("Controller"));

            foreach (var controller in controllers)
            {
                //controller need authrozie
                if (controller.CustomAttributes.Any(c => c.AttributeType == typeof(AuthorizeAttribute)))
                {
                    bool isControllerAccept = true;
                    object[] controllerAtts = controller.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                    foreach (AuthorizeAttribute att in controllerAtts)
                    {
                        if (object.ReferenceEquals(att.Policy, null))
                            continue;

                        if (!claimsList.Any(c => c.Type == "AccessLevel" &&
                         c.Value == att.Policy))
                        {
                            isControllerAccept = false;
                            break;
                        }

                    }
                    //user authorize for controller
                    if (isControllerAccept)
                    {
                        var methods = controller.GetMethods().Where(m => m.CustomAttributes.Any
                    (z => z.AttributeType == typeof(MarkedToNavBarAttribute))
                    && !m.CustomAttributes.Any(z => z.AttributeType == typeof(HttpPostAttribute))).ToList();




                        foreach (var method in methods)
                        {
                            string persianMathodName = method.GetCustomAttribute<MarkedToNavBarAttribute>().name;
                            //method need authorize
                            if (method.CustomAttributes.Any(m => m.AttributeType == typeof(AuthorizeAttribute)))
                            {
                                bool addToList = true;
                                object[] atts = method.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                                foreach (AuthorizeAttribute att in atts)
                                {
                                    if (object.ReferenceEquals(att.Policy, null))
                                        continue;

                                    if (!claimsList.Any(x => x.Type == "AccessLevel"
                                     && x.Value == att.Policy))
                                    {
                                        addToList = false;
                                        break;
                                    }
                                }
                                if (addToList)
                                    accessLevel.Add(new NavBarViewModel()
                                    {
                                        ActionName = method.Name,
                                        ControllerName = controller.Name.Remove(controller.Name.Length - 10),
                                        PersianAccessLevelName = persianMathodName
                                    });

                            }
                            //method dont need authorize
                            else
                            {

                                accessLevel.Add(new NavBarViewModel()
                                {
                                    ActionName = method.Name,
                                    ControllerName = controller.Name.Remove(controller.Name.Length - 10),
                                    PersianAccessLevelName = persianMathodName
                                });
                            }
                        }

                    }
                }
                //controller dont need authorize
                else
                {
                    //methods should be show in menu
                    var methods = controller.GetMethods().Where(m => m.CustomAttributes.Any
                    (z => z.AttributeType == typeof(MarkedToNavBarAttribute))
                    &&!m.CustomAttributes.Any(z=>z.AttributeType==typeof(HttpPostAttribute))).ToList();

                    foreach (var method in methods)
                    {
                        string persianMathodName = method.GetCustomAttribute<MarkedToNavBarAttribute>().name;
                        //method need authorize
                        if (method.CustomAttributes.Any(m => m.AttributeType == typeof(AuthorizeAttribute)))
                        {
                            bool addToList = true;
                            object[] atts = method.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                            foreach (AuthorizeAttribute att in atts)
                            {
                                if (object.ReferenceEquals(att.Policy, null))
                                    continue;

                                if (!claimsList.Any(x => x.Type == "AccessLevel"
                                 && x.Value == att.Policy))
                                {
                                    addToList = false;
                                    break;
                                }
                            }
                            if (addToList)
                                accessLevel.Add(new NavBarViewModel()
                                {
                                    ActionName = method.Name,
                                    ControllerName = controller.Name,
                                    PersianAccessLevelName = persianMathodName
                                });

                        }
                        //method dont need authorize
                        else
                        {

                            accessLevel.Add(new NavBarViewModel()
                            {
                                ActionName = method.Name,
                                ControllerName = controller.Name.Remove(controller.Name.Length - 10),
                                PersianAccessLevelName = persianMathodName
                            });
                        }
                    }
                }
            }



            //foreach (var item in controllers)
            //{
            //    bool checkControllerPolicy = true;
            //    string persianControllerAccessname = "";
            //    if (item.CustomAttributes.Any(c => c.AttributeType == typeof(AuthorizeAttribute)))
            //    {
            //        bool controllerMarkToShow = false;

            //        object[] policyAtts = item.GetCustomAttributes(typeof(AuthorizeAttribute), true);
            //        if (item.CustomAttributes.Any(c => c.AttributeType == typeof(MarkedToNavBarAttribute)))
            //        {
            //            persianControllerAccessname = item.GetCustomAttribute<MarkedToNavBarAttribute>().name;
            //            controllerMarkToShow = true;
            //        }

            //        foreach (AuthorizeAttribute attItem in policyAtts)
            //        {
            //            var policyName = attItem.Policy;
            //            if (!claimsList.Any(c => c.Type == "AccessLevel" && c.Value == policyName))
            //            {
            //                checkControllerPolicy = false;
            //            }
            //            else
            //            {
            //                if (controllerMarkToShow)
            //                {
            //                    accessLevel.Add(new NavBarViewModel()
            //                    {
            //                        ActionName = "",
            //                        ControllerName = item.Name.Remove(item.Name.Length - 10),
            //                        PersianAccessLevelName = persianControllerAccessname
            //                    });
            //                }

            //            }
            //        }

            //    }

            //    if (checkControllerPolicy)
            //    {

            //        var methodInfos = item.GetMethods().Where(x => x.CustomAttributes
            //                                                    .Any(c => c.AttributeType == typeof(MarkedToNavBarAttribute)));
            //        foreach (var methodItem in methodInfos)
            //        {
            //            bool methodMarkToShow = false;
            //            string PersianMethodAccessName = "";
            //            if (methodItem.CustomAttributes.Any(x => x.AttributeType == typeof(MarkedToNavBarAttribute)))
            //            {
            //                PersianMethodAccessName = methodItem.GetCustomAttribute<MarkedToNavBarAttribute>().name;
            //                methodMarkToShow = true;
            //            }
            //            if (methodMarkToShow)
            //            {
            //                object[] methodAtts = methodItem.GetCustomAttributes(typeof(AuthorizeAttribute), true);
            //                if (methodAtts == null || methodAtts.Length == 0)
            //                {
            //                    accessLevel.Add(new NavBarViewModel()
            //                    {
            //                        ActionName = methodItem.Name,
            //                        ControllerName = item.Name.Remove(item.Name.Length - 10),
            //                        PersianAccessLevelName = persianControllerAccessname

            //                    });
            //                }
            //                if (methodAtts.Count() == 1 && methodAtts[0] is AuthorizeAttribute)
            //                {
            //                    var justAuthorizeAtt = methodAtts[0] as AuthorizeAttribute;

            //                    if (justAuthorizeAtt.Policy == "")
            //                    {
            //                        accessLevel.Add(new NavBarViewModel()
            //                        {
            //                            ActionName = methodItem.Name,
            //                            ControllerName = item.Name.Remove(item.Name.Length - 10),
            //                            PersianAccessLevelName = PersianMethodAccessName

            //                        });
            //                    }
            //                }
            //                foreach (AuthorizeAttribute att in methodAtts)
            //                {
            //                    var policyName = att.Policy;
            //                    if (claimsList.Any(c => c.Type == "AccessLevel" && c.Value == policyName))
            //                    {

            //                        if (!accessLevel.Any(x => x.ActionName == methodItem.Name
            //                                                && x.PersianAccessLevelName == PersianMethodAccessName))
            //                        {
            //                            accessLevel.Add(new NavBarViewModel()
            //                            {
            //                                ActionName = methodItem.Name,
            //                                ControllerName = item.Name.Remove(item.Name.Length - 10),
            //                                PersianAccessLevelName = PersianMethodAccessName
            //                            });
            //                        }

            //                    }
            //                }
            //            }

            //        }
            //    }
            //}
            var groupByControllerNames = accessLevel.GroupBy(c => c.ControllerName);


            return View(groupByControllerNames);

        }

    }
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

            foreach (var controller in controllers)
            {
                //controller need authrozie
                if (controller.CustomAttributes.Any(c => c.AttributeType == typeof(AuthorizeAttribute)))
                {
                    bool isControllerAccept = true;
                    object[] controllerAtts = controller.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                    foreach (AuthorizeAttribute att in controllerAtts)
                    {
                        if (object.ReferenceEquals(att.Policy, null))
                            continue;

                        if (!claimsList.Any(c => c.Type == "AccessLevel" &&
                         c.Value == att.Policy))
                        {
                            isControllerAccept = false;
                            break;
                        }

                    }
                    //user authorize for controller
                    if (isControllerAccept)
                    {
                        var methods = controller.GetMethods().Where(m => m.CustomAttributes.Any
                    (z => z.AttributeType == typeof(MarkedToNavBarAttribute))
                    && !m.CustomAttributes.Any(z => z.AttributeType == typeof(HttpPostAttribute))).ToList();




                        foreach (var method in methods)
                        {
                            string persianMathodName = method.GetCustomAttribute<MarkedToNavBarAttribute>().name;
                            //method need authorize
                            if (method.CustomAttributes.Any(m => m.AttributeType == typeof(AuthorizeAttribute)))
                            {
                                bool addToList = true;
                                object[] atts = method.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                                foreach (AuthorizeAttribute att in atts)
                                {
                                    if (object.ReferenceEquals(att.Policy, null))
                                        continue;

                                    if (!claimsList.Any(x => x.Type == "AccessLevel"
                                     && x.Value == att.Policy))
                                    {
                                        addToList = false;
                                        break;
                                    }
                                }
                                if (addToList)
                                    accessLevel.Add(new NavBarViewModel()
                                    {
                                        ActionName = method.Name,
                                        ControllerName = controller.Name.Remove(controller.Name.Length - 10),
                                        PersianAccessLevelName = persianMathodName
                                    });

                            }
                            //method dont need authorize
                            else
                            {

                                accessLevel.Add(new NavBarViewModel()
                                {
                                    ActionName = method.Name,
                                    ControllerName = controller.Name.Remove(controller.Name.Length - 10),
                                    PersianAccessLevelName = persianMathodName
                                });
                            }
                        }

                    }
                }
                //controller dont need authorize
                else
                {
                    //methods should be show in menu
                    var methods = controller.GetMethods().Where(m => m.CustomAttributes.Any
                    (z => z.AttributeType == typeof(MarkedToNavBarAttribute))
                    && !m.CustomAttributes.Any(z => z.AttributeType == typeof(HttpPostAttribute))).ToList();

                    foreach (var method in methods)
                    {
                        string persianMathodName = method.GetCustomAttribute<MarkedToNavBarAttribute>().name;
                        //method need authorize
                        if (method.CustomAttributes.Any(m => m.AttributeType == typeof(AuthorizeAttribute)))
                        {
                            bool addToList = true;
                            object[] atts = method.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                            foreach (AuthorizeAttribute att in atts)
                            {
                                if (object.ReferenceEquals(att.Policy, null))
                                    continue;

                                if (!claimsList.Any(x => x.Type == "AccessLevel"
                                 && x.Value == att.Policy))
                                {
                                    addToList = false;
                                    break;
                                }
                            }
                            if (addToList)
                                accessLevel.Add(new NavBarViewModel()
                                {
                                    ActionName = method.Name,
                                    ControllerName = controller.Name,
                                    PersianAccessLevelName = persianMathodName
                                });

                        }
                        //method dont need authorize
                        else
                        {

                            accessLevel.Add(new NavBarViewModel()
                            {
                                ActionName = method.Name,
                                ControllerName = controller.Name.Remove(controller.Name.Length - 10),
                                PersianAccessLevelName = persianMathodName
                            });
                        }
                    }
                }
            }



            
            return View(accessLevel);
        }
    }
}
