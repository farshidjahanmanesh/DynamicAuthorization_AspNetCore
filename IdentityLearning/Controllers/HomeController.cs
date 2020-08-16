using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityLearning.Infrastructure;
using IdentityLearning.Models;
using IdentityLearning.Models.ViewModels;
using IdentityLearning.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IdentityLearning.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
            LoginViewModel v = new LoginViewModel()
            {
                Email="sadaqwe",
                Password="<script></script>",
                RememberMe=false,
                ReturnUrl=null
            };
          
        }

        public IActionResult Index([FromServices] TestIdentityDbContext _ctx)
        {
            GraphDataViewModel gp = new GraphDataViewModel(User.Claims.ToList(),_ctx);
            return View(gp);
        }

    }

  
}