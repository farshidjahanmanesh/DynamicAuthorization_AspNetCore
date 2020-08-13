using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityLearning.Infrastructure;
using IdentityLearning.Models;
using IdentityLearning.Models.ViewModels;
using IdentityLearning.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityLearning.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IMailService mailService;
        private readonly AccountService accountService;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            IMailService mailService, AccountService accountService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mailService = mailService;
            this.accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.returnUrl = login.ReturnUrl;
                return View(login);
            }
            User user = await userManager.FindByEmailAsync(login.Email);
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

            var result = await signInManager.PasswordSignInAsync(
            user, login.Password, login.RememberMe, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, AuthorizeSystemError.InvalidUsernameOrPassword);
                return View(login);
            }

            return LocalRedirect(login.ReturnUrl ?? "/");

        }

        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            var user = User;
            await signInManager.SignOutAsync();
            return RedirectToAction("index", controllerName: "home");
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            return View(model: returnUrl);
        }


        public IActionResult CreateAccount()
        {
            return View(new UserAccountViewModel());
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> CreateAccount(UserAccountViewModel uv)
        {
            if (ModelState.IsValid)
            {

                var userDetail = new User()
                {
                    PersianName = uv.PersianName,
                    Email = uv.Email,
                    UserName = uv.Email

                };
                var createResult = await userManager.CreateAsync(userDetail, uv.Password);

                if (createResult.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(uv.Email);
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var link = Url.Action("ConfirmEmail", "account", new { token = token, email = uv.Email }, Request.Scheme);
                    await userManager.AddClaimAsync(user, new Claim("UserName", uv.PersianName));
                    MailRequest mr = new MailRequest()
                    {
                        Body = link,
                        Subject = "تاییدیه ایمیل",
                        ToEmail = uv.Email
                    };

                    await mailService.SendEmailAsync(mr);
                    return RedirectToAction("login", "account");
                }

                foreach (var item in createResult.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);
                }

            }
            return View(uv);


        }

        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            var userPassword = new ChangePasswrodViewModel()
            {
                UserId = user.Id,
                ConfirmPassword = "",
                OldPassword = "",
                Password = ""
            };
            return View(userPassword);
        }

        [HttpPost]
        [Authorize]
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

            return View(cp);
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
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

        [Authorize]
        public IActionResult ResetPassword(string email, string token)
        {
            var fp = new ForgetPasswordViewModel()
            {
                Token = token,
                Email = email
            };
            return View(fp);
        }

        [Authorize]
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


            throw new System.Exception();
        }

        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
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


        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account");
            var result = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", result);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction(nameof(Login));
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

            if (result.Succeeded)
                return RedirectToAction("index", "home");

            var userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                var createUser = new User
                {
                    Email = userEmail,
                    UserName = userEmail,

                };
                var pass =string.Concat(Guid.NewGuid().ToString().Take(10));
                var createResult = await userManager.CreateAsync(createUser, pass);
                if (!createResult.Succeeded)
                    return NotFound();

                user = await userManager.FindByEmailAsync(userEmail);
                await signInManager.SignInAsync(user, true);
                

                var loginResult = await userManager.AddLoginAsync(user, info);

                try
                {
                    var bodyRequest = new StringBuilder();
                    bodyRequest.AppendLine($"حساب کاربری شما با ایمیل {pass} و رمز عبور {userEmail} در سایت فعال شده است.");
                    bodyRequest.AppendLine("از این پس میتوانید علاوه بر ورود به سایت از طریق حساب کاربری جیمیل ، از این طریق نیز به سایت وارد شوید");
                    bodyRequest.AppendLine("با تشکر از شما.");
                    var mail = new MailRequest()
                    {
                        Subject = "حساب کاربری فعال شد",
                        ToEmail = userEmail,
                        Body = bodyRequest.ToString()
                    };
                    await mailService.SendEmailAsync(mail);
                }
                catch (Exception)
                {

                }
              
                return RedirectToAction("index", "home");
            }
            else
            {
                var loginResult = await userManager.AddLoginAsync(user, info);
                if (loginResult.Succeeded)
                {
                    await signInManager.SignInAsync(user, true);
                    return RedirectToAction("Index", "Home");
                }
            }

            return NotFound();

        }

        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            var userModel = new UserUpdateViewModel()
            {
                AboutMe = user.AboutMe,
                Email = user.Email,
                Id = user.Id,
                PersianName = user.PersianName,
                PhoneNumber = user.PhoneNumber
            };

            return View(userModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(UserUpdateViewModel up)
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
            var takePersianName = user.PersianName;
            user.PersianName = up.PersianName;
            user.AboutMe = up.AboutMe;
            user.PhoneNumber = up.PhoneNumber;

            var updateUser = await userManager.UpdateAsync(user);
            if (!updateUser.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "پردازش سیستم با مشکل مواجه شده است");
                return View(up);
            }
            ViewData["success"] = true;
            var r = await userManager.ReplaceClaimAsync(user, new Claim("UserName", takePersianName),
                new Claim("UserName", user.PersianName));

            return View(up);

            throw new NotImplementedException();
        }
    }
}