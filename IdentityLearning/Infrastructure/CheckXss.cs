
using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityLearning.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CheckXssAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Type type = validationContext.ObjectType;
            value = CheckXss.CheckObject(type,value);
            return ValidationResult.Success;
        }

    }

    public static class CheckXss
    {
        public static string CheckIt(string value) => Sanitizer.GetSafeHtmlFragment(value);


        public static T CheckObject<T>(Type type,T obj) where T : class
        {

            var properties = type.UnderlyingSystemType.GetProperties().Where(c => c.PropertyType == typeof(string));
            foreach (var prop in properties)
            {
                string value = prop.GetValue(obj) as string;
                if (object.ReferenceEquals(value, null))
                    continue;
                value = CheckIt(value);
                prop.SetValue(obj, value);
            }
            return obj;
        }


       


    }
}
