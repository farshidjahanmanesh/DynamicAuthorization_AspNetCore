using IdentityLearning.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdentityLearning.Controllers
{
    [Policy(persmission.BasePermission)]
    public class PostController : Controller
    {
        [Policy(persmission.AddPostPermission)]
        public IActionResult CreatePost() => Ok();

        [Policy(persmission.UpdatePostPermission )]
        public IActionResult UpdatePost() => Ok();

        [Policy(persmission.RemovePostPermission )]

        public IActionResult DeletePost() => Ok();
    }
}