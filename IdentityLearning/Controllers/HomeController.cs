using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityLearning.Infrastructure;
using IdentityLearning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityLearning.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }
        public IActionResult Index()
        {
            return View(User.Claims);
        }

    }

  
}