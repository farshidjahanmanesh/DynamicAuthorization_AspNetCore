using IdentityLearning.Infrastructure;
using IdentityLearning.Models;
using IdentityLearning.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityLearning.Controllers
{
    [Policy(persmission.BasePermission)]
    public class UserManagerController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserManagerController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;

        }



        [Policy(persmission.UserView)]
        [MarkedToNavBar("نمایش کاربران")]
        public IActionResult UsersView()
        {
            if (TempData.ContainsKey("DeleteSucc"))
            {
                TempData.Remove("DeleteSucc");
                ViewData["DeleteSucc"] = true;
            }
            else if (TempData.ContainsKey("DeleteError"))
            {
                TempData.Remove("DeleteError");
                ViewData["DeleteError"] = true;
            }
            var users = userManager.Users.Where(c => c.UserName != User.Identity.Name).ToList();
            return View(users);
        }

        [ValidateAntiForgeryToken]
        [Policy(persmission.DeleteUser)]
        [ServiceFilter(typeof(IsUserExistFilter))]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(id);
                await userManager.DeleteAsync(user);
                TempData["DeleteSucc"] = true;
            }
            else
            {
                TempData["DeleteError"] = true;
            }

            return RedirectToAction(nameof(UsersView));
        }


        [Policy(persmission.UpdateUser)]
        [ServiceFilter(typeof(IsUserExistFilter))]
        public async Task<IActionResult> UpdateUser(string id)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(id);
                UserUpdateViewModel up = new UserUpdateViewModel()
                {
                    AboutMe = user.AboutMe,
                    PersianName = user.PersianName,
                    Email = user.Email,
                    Id = user.Id,
                    PhoneNumber = user.PhoneNumber
                };
                return View(up);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Policy(persmission.UpdateUser)]
        [ServiceFilter(typeof(IsUserExistFilter))]
        public async Task<IActionResult> UpdateUser(UserUpdateViewModel userUpdate)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(userUpdate.Id);
                bool result = false;
                bool isEmailNew = userUpdate.Email.ToLower() != user.Email;
                if (isEmailNew)
                {
                    result = userManager.Users.Any(x => x.Email.ToLower() == userUpdate.Email.ToLower()
                                       || x.UserName.ToLower() == userUpdate.Email.ToLower());

                }

                if (result == false)
                {

                    user.AboutMe = userUpdate.AboutMe;
                    user.PersianName = userUpdate.PersianName;
                    user.PhoneNumber = userUpdate.PhoneNumber;
                    if (isEmailNew)
                    {
                        user.Email = userUpdate.Email;
                        user.UserName = userUpdate.Email;
                    }
                    var updateResult = await userManager.UpdateAsync(user);
                    if (updateResult.Succeeded == false)
                    {
                        ModelState.AddModelError("errorInProcess", "سیستم با مشکل مواجه شده است");
                        return View(userUpdate);
                    }
                }
                else
                {
                    ModelState.AddModelError("EmailIsDuplicated", "ایمیل وارد شده تکراری است");
                    return View(userUpdate);
                }
                return RedirectToAction(nameof(UsersView));
            }

            return View(userUpdate);

        }


        [Policy(persmission.SetRoleTOUser)]
        [ServiceFilter(typeof(IsUserExistFilter))]
        public async Task<IActionResult> AccessLevelUser(string id)
        {
            if (ModelState.IsValid)
            {
                if (TempData["Success"]!=null)
                {
                    TempData.Remove("Success");
                    ViewBag.Success = true;
                }
                var roles = roleManager.Roles.ToList();
                var user = await userManager.FindByIdAsync(id);
                var userRolenames = await userManager.GetRolesAsync(user);
                var userRoles = roleManager.Roles.Where(x => userRolenames.Any(y => y == x.Name)).ToList();
                UserToRoleViewModel uv = new UserToRoleViewModel();
                uv.UserId = user.Id;
                uv.UserName = user.PersianName;

                foreach (var item in userRoles)
                {
                    uv.RoleIn.Add(new SelectListItem()
                    {
                        Selected = true,
                        Value = item.Id,
                        Text = item.Name
                    });
                }

                foreach (var item in roles.Except(userRoles).ToList())
                {
                    uv.RoleOut.Add(new SelectListItem()
                    {
                        Selected = false,
                        Text = item.Name,
                        Value = item.Id
                    });
                }

                return View(uv);

            }
            return NotFound();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Policy(persmission.SetRoleTOUser)]
        public async Task<IActionResult> AccessLevelUser(UserToRoleViewModel uv)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(uv.UserId);
                if (user == null)
                {
                    ModelState.AddModelError("IdNotValid", "چنین کاربری در سیستم موجود نیست");
                    ViewBag.UnValid = true;
                    return View(uv);
                }
                var roleMustRemove = new List<string>();
                foreach(var item in uv.RoleIn)
                {
                    if (!item.Selected)
                        roleMustRemove.Add(item.Text);
                }

                var roleMustAdd = new List<string>();
                foreach (var item in uv.RoleOut)
                {
                    if (item.Selected)
                        roleMustAdd.Add(item.Text);
                }


                var addResult = await userManager.AddToRolesAsync(user,roleMustAdd);
                var subresult = await userManager.RemoveFromRolesAsync(user,roleMustRemove);
                
                if (!addResult.Succeeded||!subresult.Succeeded)
                {
                    ModelState.AddModelError("errorInProcess", "پردازش این عملیات با مشکل مواجه شده است");
                    ViewBag.UnValid = true;
                    return View(uv);
                }
                TempData["Success"] = true;
                return RedirectToAction("AccessLevelUser", new[] { uv.UserId });
            }
            ViewBag.UnValid = true;
            return View(uv);

        }
    }
}