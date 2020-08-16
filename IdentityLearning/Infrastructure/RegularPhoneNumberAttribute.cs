using Backend_React;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IdentityLearning.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RegularPhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string)
            {
                var phoneNumber = value as string;
                Regex enRegex = new Regex(@"(\+98|0)?9\d{9}");
                var enResult = enRegex.Match(phoneNumber).Success;
                if (enResult)
                    return true;
                Regex faRegix = new Regex(@"(\+۹۸|۰)?۹\d{9}");
                var faResult = faRegix.Match(phoneNumber).Success;
                var z = 1;
                if (faResult)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
