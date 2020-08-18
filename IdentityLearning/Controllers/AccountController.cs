using AutoMapper;
using Backend_React;
using IdentityLearning.Infrastructure;
using IdentityLearning.Models.ViewModels;
using IdentityLearning.Services;
using MailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedServices.Models.IdentityModels;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UploadService;

namespace IdentityLearning.Controllers
{
    // [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMailService mailService;
        private readonly IMapper mapper;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IMailService mailService, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mailService = mailService;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [MarkedToNavBar("ورود با حساب کاربری جدید")]
        public IActionResult Login(string returnUrl)
        {
            returnUrl = CheckXss.CheckIt(returnUrl);
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.returnUrl = login.ReturnUrl;
                return View(login);
            }


            var user = await userManager.FindByEmailAsync(login.Email);
            if (object.ReferenceEquals(null, user))
            {
                ModelState.AddModelError(string.Empty, AuthorizeSystemError.InvalidUsernameOrPassword);
                return View(login);
            }


            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                return View("EmailConfirm");
            }
            await signInManager.SignOutAsync();

            if (!user.IsActive)
                return View("ActivateAccount");

            var result = await signInManager.PasswordSignInAsync(
            user, login.Password, login.RememberMe, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, AuthorizeSystemError.InvalidUsernameOrPassword);
                return View(login);
            }

            return LocalRedirect(login.ReturnUrl ?? "/");

        }

        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", controllerName: "home");
        }


        public IActionResult AccessDenied(string returnUrl)
        {
            returnUrl = CheckXss.CheckIt(returnUrl);
            return View(model: returnUrl);
        }


        [AllowAnonymous]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> CreateAccount(UserAccountViewModel uv)
        {
            if (ModelState.IsValid)
            {
                var userDetail = mapper.Map<ApplicationUser>(uv);
                userDetail.IsActive = true;
                userDetail.IsExternalLogin = false;

                var createResult = await userManager.CreateAsync(userDetail, uv.Password.Trim());

                if (createResult.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(uv.Email);
                    await userManager.AddClaimAsync(user, new Claim("UserName", uv.PersianName.Trim()));
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var link = Url.Action("ConfirmEmail", "account", new { token = token, email = uv.Email }, Request.Scheme);
                    MailRequest mr = new MailRequest()
                    {
                        Body = link,
                        Subject = "تاییدیه ایمیل",
                        ToEmail = uv.Email
                    };

                    await mailService.SendEmailAsync(mr);
                    return View("EmailConfirm");
                }
                var persianErrors = AuthorizeSystemError.GetErrors(createResult.Errors);
                if (persianErrors != null)
                    foreach (var item in persianErrors)
                    {
                        if (item.Code != "DuplicateUserName")
                            ModelState.AddModelError(item.Code, item.Description);
                    }

            }
            return View(uv);


        }

        [MarkedToNavBar("تغییر رمز عبور")]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            var userPassword = mapper.Map<ChangePasswrodViewModel>(user);
            return View(userPassword);
        }

        [HttpPost]
        [MarkedToNavBar("تغییر رمز عبور")]
        public async Task<IActionResult> ChangePassword(ChangePasswrodViewModel cp)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var changeResult = await userManager.ChangePasswordAsync(user, cp.OldPassword, cp.Password);
                if (changeResult.Succeeded)
                {
                    ViewData["success"] = true;
                    return View(new ChangePasswrodViewModel()
                    {
                        UserId = cp.UserId
                    });
                }

                foreach (var item in changeResult.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);

                }

            }
            //for when user dont have password
            //becuase using external login
            if (cp.OldPassword == null || cp.OldPassword == "")
                if (ModelState.ErrorCount == 1)
                {
                    var oldPassState = ModelState[nameof(cp.OldPassword)];
                    //just oldpassword have error
                    if (oldPassState.Errors.Count == 1)
                    {
                        var user = await userManager.FindByIdAsync(cp.UserId);
                        if (object.ReferenceEquals(user.PasswordHash, null))
                        {
                            var changePasswordResult = await userManager.AddPasswordAsync(user, cp.Password);
                            if (changePasswordResult.Succeeded)
                                ViewData["success"] = true;
                            else
                                ViewData["faild"] = true;
                            ModelState.Clear();
                            return View(cp);
                        }
                    }
                }


            return View(cp);
        }

        [AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            email = CheckXss.CheckIt(email);
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewData["NotFound"] = true;
                return View();
            }
            string token = await userManager.GeneratePasswordResetTokenAsync(user);
            var callBack = Url.Action("ResetPassword", "Account", new { token = token, email = email }, Request.Scheme);
            MailRequest message = new MailRequest()
            {
                Subject = "درخواست تغییر رمز عبور",
                ToEmail = email,
                Body = $" اگر شما درخواست تغییر رمز عبور را داده اید ، بر روی لینک {callBack} کلیک کنید"
            };
            await mailService.SendEmailAsync(message);
            ViewData["Success"] = true;
            return View();
        }

        [AllowAnonymous]
        //   [Authorize]
        public IActionResult ResetPassword(string email, string token)
        {
            email = CheckXss.CheckIt(email);
            token = CheckXss.CheckIt(token);
            var fp = new ForgetPasswordViewModel()
            {
                Token = token,
                Email = email
            };
            return View(fp);
        }

        [AllowAnonymous]
        //   [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ForgetPasswordViewModel fp)
        {
            if (!ModelState.IsValid)
                return View(fp);

            var user = await userManager.FindByEmailAsync(fp.Email);
            if (user == null)
            {
                return NotFound();
            }

            var resetPassResult = await userManager.ResetPasswordAsync(user, fp.Token, fp.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return View();
            }

            return RedirectToAction("index", "home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            email = CheckXss.CheckIt(email);
            token = CheckXss.CheckIt(token);
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound();

            var confirmResult = await userManager.ConfirmEmailAsync(user, token);
            if (confirmResult.Succeeded)
            {
                await signInManager.SignInAsync(user, true);
                return RedirectToAction("index", "home");

            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in confirmResult.Errors)
            {
                sb.AppendLine(item.Description);
            }
            return Content(sb.ToString());

        }

        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account");
            var result = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", result);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction(nameof(Login));
            var userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(userEmail);

            if (user != null)
                if (!user.IsActive)
                    return View("ActivateAccount");

            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

            if (result.Succeeded)
            {
                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                }

                if (user != null)
                    if (!user.IsActive)
                        return View("ActivateAccount");

                return RedirectToAction("index", "home");
            }



            if (user == null)
            {
                var createUser = new ApplicationUser
                {
                    Email = userEmail,
                    UserName = userEmail,
                    IsExternalLogin = true,
                    IsActive = true,
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(createUser);
                if (!createResult.Succeeded)
                    return NotFound();

                user = await userManager.FindByEmailAsync(userEmail);
                await signInManager.SignInAsync(user, true);



                var loginResult = await userManager.AddLoginAsync(user, info);

                return RedirectToAction("index", "home");
            }
            else
            {
                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                }


                var loginResult = await userManager.AddLoginAsync(user, info);
                if (loginResult.Succeeded)
                {
                    await signInManager.SignInAsync(user, true);
                    return RedirectToAction("Index", "Home");
                }
            }

            return NotFound();

        }

        [MarkedToNavBar("تغییر پروفایل کاربری")]
        public async Task<IActionResult> EditProfile()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            var userModel = mapper.Map<UserUpdateViewModel>(user);
            userModel.ProfileImageUrl = string.Format("/images/profiles/{0}", user.ProfilePicture);
            return View(userModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UserUpdateViewModel up, [FromServices]FileUpload fileUpload)
        {
            if (!ModelState.IsValid)
                return View(up);

            var user = await userManager.FindByIdAsync(up.Id);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "ای دی شما در سیستم موجود نیست");
                return View(up);
            }
            if (user.Email.ToLower() != up.Email.ToLower())
            {
                var changeEmailResult = await userManager.SetEmailAsync(user, up.Email);
                if (!changeEmailResult.Succeeded)
                {
                    foreach (var item in changeEmailResult.Errors)
                        ModelState.AddModelError(string.Empty, item.Description);
                    return View(up);
                }

            }
            var takePersianName = user.PersianName.Trim();
            var takeProfilePicture = user.ProfilePicture;
            user.PersianName = up.PersianName.Trim();
            user.AboutMe = up.AboutMe?.Trim();
            user.PhoneNumber = up.PhoneNumber.Fa2En();

            string locUplaod = fileUpload.UploadedProfile(up.ProfileImage, takeProfilePicture);
            user.ProfilePicture = locUplaod;
            up.ProfileImageUrl = locUplaod;
            var updateUser = await userManager.UpdateAsync(user);
            if (!updateUser.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "پردازش سیستم با مشکل مواجه شده است");
                return View(up);
            }
            ViewData["success"] = true;
            if (user.ProfilePicture == null)
            {
                if (string.IsNullOrWhiteSpace(takeProfilePicture))
                    await userManager.RemoveClaimAsync(user, new Claim("ProfilePicture", takeProfilePicture));
            }
            else
            {

                if (string.IsNullOrWhiteSpace(takeProfilePicture))
                {
                    var r = await userManager.AddClaimAsync(user,
                  new Claim("ProfilePicture", user.ProfilePicture));
                }
                else
                {
                    var r = await userManager.ReplaceClaimAsync(user, new Claim("ProfilePicture", takeProfilePicture),
                   new Claim("ProfilePicture", user.ProfilePicture));
                }

            }

            if (string.IsNullOrWhiteSpace(takePersianName))
            {
                var r = await userManager.AddClaimAsync(user,
               new Claim("UserName", user.PersianName));

            }
            else
            {
                var r = await userManager.ReplaceClaimAsync(user, new Claim("UserName", takePersianName),
               new Claim("UserName", user.PersianName));

            }

            await signInManager.RefreshSignInAsync(user);
            return View(up);

        }

        [HttpPost]
        public async Task<JsonResult> ChangeUserActivate([FromBody]string userId)
        {
            userId = CheckXss.CheckIt(userId);
            var user = await userManager.FindByIdAsync(userId);
            if (object.ReferenceEquals(user, null))
                return Json("failed");
            user.IsActive = user.IsActive == true ? false : true;
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Json("failed");

            return Json("success");
        }
    }
}