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
    public static class AuthorizeSystemError
    {
        public const string DuplicateEmail = "ایمیل تکراری است";
        public const string InvalidEmail = "ایمیل نامعتبر است";
        public const string DuplicateUserName = "نام کاربری تکراری است";
        public const string InvalidUserName = "نام کاربری نامعتبر است";

        public const string PasswordTooShort = "رمز عبور کوتاه است";
        public const string PasswordRequiresNonAlphanumeric = "رمز عبور نیاز به حروف غیر الفبا دارد";
        public const string PasswordRequiresDigit = "رمز عبور نیاز به عدد دارد";
        public const string PasswordRequiresLower = "رمز عبور نیاز به کارکتر هایی با حروف کوچک دارد";
        public const string PasswordRequiresUpper = "رمز عبور نیاز به کارکتر هایی با حروف بزرگ دارد";

        public const string EmailIsNotConfirm = "ایمیل تایید نشده است";
        public const string InvalidUsernameOrPassword = "نام کاربری یا رمز عبور اشتباه است";

        static AuthorizeSystemError()
        {

            var fields = typeof(AuthorizeSystemError).GetFields().ToList();
            foreach (var field in fields)
            {
                Codes.Add(field.Name);
            }
        }

        public static List<string> Codes { get; set; } = new List<string>();

        public static IdentityError[] GetErrors(IEnumerable<IdentityError> errors)
        {
            IdentityError[] result = new IdentityError[errors.Count()];
            int counter = 0;
            foreach (var item in errors)
            {
                if (AuthorizeSystemError.Codes.Any(x => x == item.Code))
                {
                    var fieldName = (string)typeof(AuthorizeSystemError).GetField(item.Code).GetValue(null);
                    item.Description = fieldName;
                    result[counter]=item;
                    counter++;
                }
                else
                {
                    result[counter]=item;
                    counter++;
                }
            }

            return result;
        }


    }


    //public class AccountService
    //{
    //    public enum AccountServiceError
    //    {
    //        email_is_not_confirm,
    //        invalid_username_or_password,
    //        DuplicateEmail,
    //        PasswordTooShort,
    //        PasswordRequiresLower,
    //        InvalidEmail,
    //        DuplicateUserName,
    //        InvalidUserName,
    //    }


    //    private readonly UserManager<User> userManager;
    //    private readonly SignInManager<User> signInManager;
    //    private readonly IMailService mailService;
    //    // private readonly IUrlHelper url;

    //    public AccountService(UserManager<User> userManager, SignInManager<User> signInManager,
    //        IMailService mailService)
    //    {
    //        this.userManager = userManager;
    //        this.signInManager = signInManager;
    //        this.mailService = mailService;

    //    }


    //    public async Task<IdentityResult> LoginAsync(LoginViewModel loginDetail)
    //    {
    //        User user = await userManager.FindByEmailAsync(loginDetail.Email);
    //        if (user != null)
    //        {
    //            if (!await userManager.IsEmailConfirmedAsync(user))
    //            {
    //                return IdentityResult.Failed(new IdentityError()
    //                {
    //                    Code = nameof(AccountServiceError.email_is_not_confirm),
    //                    Description = "email is not comfirmed"
    //                });
    //            }

    //            await signInManager.SignOutAsync();

    //            var result =
    //            await signInManager.PasswordSignInAsync(
    //            user, loginDetail.Password, loginDetail.RememberMe, false);
    //            if (result.Succeeded)
    //            {
    //                return IdentityResult.Success;
    //            }
    //        }
    //        return IdentityResult.Failed(new IdentityError()
    //        {
    //            Code = nameof(AccountServiceError.invalid_username_or_password),
    //            Description = "invalid username or password"
    //        });
    //    }

    //    public async Task<IdentityResult> CreateAccount(UserAccountViewModel userInfo, string link)
    //    {
    //        var userDetail = new User()
    //        {
    //            PersianName = userInfo.PersianName,
    //            Email = userInfo.Email,
    //            UserName = userInfo.Email

    //        };
    //        var createResult = await userManager.CreateAsync(userDetail, userInfo.Password);

    //        if (createResult.Errors.Count() > 0)
    //        {
    //            IdentityError[] errors = GetErrors(createResult.Errors);
    //            return IdentityResult.Failed(errors);
    //        }
    //        var user = await userManager.FindByEmailAsync(userInfo.Email);
    //        if (link == null || link == "")
    //        {
    //            MailRequest mr = new MailRequest()
    //            {
    //                Body = link,
    //                Subject = "تاییدیه ایمیل",
    //                ToEmail = userInfo.Email
    //            };
    //            await mailService.SendEmailAsync(mr);
    //        }
    //        await userManager.AddClaimAsync(user, new Claim("UserName", userInfo.PersianName));


    //        return IdentityResult.Success;



    //    }


    //    private IdentityError[] GetErrors(IEnumerable<IdentityError> errors)
    //    {
    //        //List<IdentityError> result = new List<IdentityError>();
    //        IdentityError[] result = new IdentityError[errors.Count()];

    //        foreach (var item in errors)
    //        {
    //            if (AuthorizeSystemError.Codes.Any(x => x == item.Code))
    //            {
    //                var fieldName = (string)typeof(AuthorizeSystemError).GetField(item.Code).GetValue(null);
    //                item.Description = fieldName;
    //                result.Append(item);
    //            }
    //            else
    //            {
    //                result.Append(item);
    //            }
    //        }

    //        return result;
    //    }

    //}
}
