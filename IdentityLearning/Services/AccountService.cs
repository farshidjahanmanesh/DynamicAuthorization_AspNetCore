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

}
