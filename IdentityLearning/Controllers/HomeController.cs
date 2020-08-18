using Microsoft.AspNetCore.Mvc;
using SharedServices.GraphModel;

namespace IdentityLearning.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        public IActionResult Index([FromServices] GraphRepository graphRepository)
        {
            graphRepository.StartGetData(User.Claims);
            return View(graphRepository);
        }

    }


}