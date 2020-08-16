using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IdentityLearning.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return Content("سیستم با ارور مواجه شده است");
        }
    }
}