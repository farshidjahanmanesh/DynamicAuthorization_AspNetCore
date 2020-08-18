using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityLearning.Models;
using IdentityLearning.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using IdentityLearning.Infrastructure;
using SharedServices.Models.IdentityModels;

namespace IdentityLearning.Controllers
{


    [Authorize(Policy = nameof(PersmissionsEnum.BasePermission))]
    public class RoleManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleManagerController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [Authorize(Policy = nameof(PersmissionsEnum.RoleView))]
        [MarkedToNavBar("نمایش نقش ها")]
        public IActionResult ShowRoles()
        {
            var roles = roleManager.Roles.Where(c=>c.Name!= "SuperAdmin").ToList();
            return View(roles);
        }

        [HttpGet]
        [Authorize(Policy = nameof(PersmissionsEnum.CreateRole))]
        [MarkedToNavBar("ایجاد یک نقش")]
        public IActionResult CreateRole()
        {
            ViewBag.permissions = ClaimToPermission.GetAllClaims();
            CreateRoleViewModel vm = new CreateRoleViewModel();
            foreach (var item in ClaimToPermission.GetAllClaims())
            {
                vm.PermissionOut.Add(new SelectListItem()
                {
                    Selected = false,
                    Text = item.Value,
                    Value = item.Key
                });
            }
            return View(vm);
        }


        [Authorize(Policy = nameof(PersmissionsEnum.CreateRole))]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                if (await roleManager.RoleExistsAsync(role.RoleName))
                {
                    ModelState.AddModelError("RoleIsExists", "نقش با این نام در سیستم موجود است.لطفا نام دیگری را امتحان کنید");
                    ViewBag.UnValid = true;
                    return View(role);

                }
                var CreateResult = await roleManager.CreateAsync(new IdentityRole()
                {
                    Name = role.RoleName
                });
                if (CreateResult.Succeeded)
                {
                    var RoleEntity = await roleManager.FindByNameAsync(role.RoleName);
                    foreach (var item in role.PermissionIn)
                    {
                        if (Enum.IsDefined(typeof(PersmissionsEnum), item.Value))
                        {
                            if (item.Selected == false)
                                await roleManager.RemoveClaimAsync(RoleEntity, new Claim("AccessLevel", item.Value));
                        }
                    }

                    foreach (var item in role.PermissionOut)
                    {
                        if (Enum.IsDefined(typeof(PersmissionsEnum), item.Value))
                        {
                            if (item.Selected == true)
                                await roleManager.AddClaimAsync(RoleEntity, new Claim("AccessLevel", item.Value));
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("ErrorInCreateRole", "ایجاد نقش با مشکل مواجه شده است");
                    ViewBag.UnValid = true;
                    return View(role);
                }

                return RedirectToAction("ShowRoles");
            }
            ViewBag.UnValid = true;
            return View(role);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(PersmissionsEnum.DeleteRole))]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            Id = CheckXss.CheckIt(Id);
            var Role = await roleManager.FindByIdAsync(Id);
            if (Role != null)
            {
                var DeleteResult = await roleManager.DeleteAsync(Role);

            }

            return RedirectToAction("ShowRoles");
        }

        [HttpGet]
        [Authorize(Policy = nameof(PersmissionsEnum.AccessLevelRole))]
        public async Task<IActionResult> AccessLevelRole(string Id)
        {
            Id = CheckXss.CheckIt(Id);
            if (TempData["success"] != null)
            {
                ViewBag.Success = true;
                TempData.Remove("success");
            }
                

            var Role = await roleManager.FindByIdAsync(Id);
            var RoleClaims = await roleManager.GetClaimsAsync(Role);


            var permissions = ClaimToPermission.GetAllClaims();
            CreateRoleViewModel vm = new CreateRoleViewModel();
            vm.RoleName = Role.Name;
            foreach (var item in RoleClaims)
            {
                var PersianName = permissions.FirstOrDefault(x => x.Key == item.Value).Value;
                if (PersianName == null || PersianName == "")
                    PersianName = item.Value;
                vm.PermissionIn.Add(new SelectListItem()
                {
                    Selected = true,
                    Text = PersianName,
                    Value = item.Value
                });
            }
            var ExceptsPermissions = permissions.Where(x => RoleClaims.Any(y => y.Value == x.Key) == false).ToList();

            foreach (var item in ExceptsPermissions)
            {
                vm.PermissionOut.Add(new SelectListItem()
                {
                    Selected = false,
                    Text = item.Value,
                    Value = item.Key
                });
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(PersmissionsEnum.AccessLevelRole))]
        public async Task<IActionResult> AccessLevelRole(CreateRoleViewModel Role)
        {
            if (ModelState.IsValid)
            {
                if (!await roleManager.RoleExistsAsync(Role.RoleName))
                {
                    ModelState.AddModelError("RoleIsNotExists", "نقشی با این نام در سیستم موجود نیست");
                    ViewBag.UnValid = true;
                    return View(Role);

                }


                var RoleEntity = await roleManager.FindByNameAsync(Role.RoleName);
                if (RoleEntity != null)
                {

                    foreach (var item in Role.PermissionIn)
                    {
                        if (Enum.IsDefined(typeof(PersmissionsEnum), item.Value))
                        {
                            if (item.Selected == false)
                            {
                                await roleManager.RemoveClaimAsync(RoleEntity, new Claim("AccessLevel", item.Value));

                            }
                        }
                    }

                    foreach (var item in Role.PermissionOut)
                    {
                        if (Enum.IsDefined(typeof(PersmissionsEnum), item.Value))
                        {
                            if (item.Selected == true)
                            {
                                await roleManager.AddClaimAsync(RoleEntity, new Claim("AccessLevel", item.Value));

                            }
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError("ErrorInCreateRole", "ایجاد نقش با مشکل مواجه شده است");
                    ViewBag.UnValid = true;

                    return View(Role);
                }
                TempData["Success"] = true;

                return RedirectToAction("AccessLevelRole", new[] { RoleEntity.Id });
            }
            ViewBag.UnValid = true;
            return View(Role);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRole(string Id)
        {
            Id = CheckXss.CheckIt(Id);
            var RoleEntity = await roleManager.FindByIdAsync(Id);
            if (RoleEntity == null)
                return NotFound();
            RoleUpdateViewModel vm = new RoleUpdateViewModel()
            {
                OldRoleName = RoleEntity.Name,
                RoleId = Id
            };
            TempData["RoleUpdatename"] = RoleEntity.Name;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole([Bind("RoleId , NewRoleName")]RoleUpdateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (await roleManager.RoleExistsAsync(vm.NewRoleName))
                {
                    ModelState.AddModelError("Name Is Exists", "نقشی با این نام در سیستم وجود دارد.نام دیگری را انتخاب کنید");

                    vm.OldRoleName = TempData["RoleUpdatename"] as string ?? "";
                    return View(vm);
                }
                var RoleEntity = await roleManager.FindByIdAsync(vm.RoleId);
                if (RoleEntity == null)
                {
                    ModelState.AddModelError("Id Is False", "چنین نقشی در سیستم موجود نیست");

                    vm.OldRoleName = TempData["RoleUpdatename"] as string ?? "";
                    return View(vm);
                }
                RoleEntity.Name = vm.NewRoleName;
                await roleManager.UpdateAsync(RoleEntity);
                return RedirectToAction(nameof(ShowRoles));
            }
            vm.OldRoleName = TempData["RoleUpdatename"] as string ?? "";
            return View(vm);
        }
    }
}