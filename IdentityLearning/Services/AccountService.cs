using IdentityLearning.Infrastructure;
using IdentityLearning.Models;
using IdentityLearning.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
namespace IdentityLearning.Services
{



    public class AccountService
    {
        public enum AccountServiceError
        {
            email_is_not_confirm,
            invalid_username_or_password
        }


        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IMailService mailService;
        private readonly IUrlHelper url;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager,
            IMailService mailService, IUrlHelper url)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mailService = mailService;
            this.url = url;
        }


        public async Task<IdentityResult> LoginAsync(LoginViewModel loginDetail)
        {
            User user = await userManager.FindByEmailAsync(loginDetail.Email);
            if (user != null)
            {
                if (!await userManager.IsEmailConfirmedAsync(user))
                {
                    return IdentityResult.Failed(new IdentityError()
                    {
                        Code = nameof(AccountServiceError.email_is_not_confirm),
                        Description = "email is not comfirmed"
                    });
                }

                await signInManager.SignOutAsync();

                SignInResult result =
                await signInManager.PasswordSignInAsync(
                user, loginDetail.Password, loginDetail.RememberMe, false);
                if (result.Succeeded)
                {
                    return IdentityResult.Success;
                }
            }
            return IdentityResult.Failed(new IdentityError()
            {
                Code = nameof(AccountServiceError.invalid_username_or_password),
                Description = "invalid username or password"
            });
        }

        public async Task<IdentityResult> CreateAccount(UserAccountViewModel userInfo,string protocol)
        {
            var userDetail = new User()
            {
                PersianName = userInfo.PersianName,
                Email = userInfo.Email,
                UserName = userInfo.Email

            };
            var createResult = await userManager.CreateAsync(userDetail, userInfo.Password);
            if (createResult.Errors.Count() > 0)
            {

            }

            if (createResult.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(userInfo.Email);
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                
                var link = url.Action("ConfirmEmail", "account", new { token = token, email = userInfo.Email }, protocol);
                await userManager.AddClaimAsync(user, new Claim("UserName", userInfo.PersianName));
                MailRequest mr = new MailRequest()
                {
                    Body = link,
                    Subject = "تاییدیه ایمیل",
                    ToEmail = userInfo.Email
                };

                await mailService.SendEmailAsync(mr);
                return IdentityResult.Success;
               // return RedirectToAction("login", "account");
            }
            
            
        }

    }
}
