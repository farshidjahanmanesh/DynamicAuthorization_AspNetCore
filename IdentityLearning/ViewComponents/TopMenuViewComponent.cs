using IdentityLearning.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using IdentityLearning.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using SharedServices.Models.IdentityModels;

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



            var groupByControllerNames = accessLevel.GroupBy(c => c.ControllerName);


            return View(groupByControllerNames);

        }

    }
}
