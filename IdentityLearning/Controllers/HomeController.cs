using System.Collections.Generic;
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
        private readonly AccountService ca;

        public HomeController(AccountService ca)
        {
           var r= AuthorizeSystemError.Codes;
           
            this.ca = ca;
           
        }

        [Authorize(Policy =nameof(IdentityLearning.Models.persmission.UserView))]
        public void temp()
        {

        }
        public async Task test()
        {
            await ca.CreateAccount(new Models.ViewModels.UserAccountViewModel()
            {
                ConfirmPassword = "12341213123",
                Email = "samicancel2@gmail.com",
                Password = "12341213123",
                PersianName = "az"
            }, "");
        }
        public IActionResult Index()
        {
            return View(User.Claims);
        }

    }

  
}