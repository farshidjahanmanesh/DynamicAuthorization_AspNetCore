using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using IdentityLearning.Infrastructure;
using IdentityLearning.Models;
using IdentityLearning.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityLearning.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMailService mail;

        public HomeController(IMailService mail)
        {
            this.mail = mail;
            
        }

        public IActionResult Index()
        {
            return View();
        }

    }

  
}