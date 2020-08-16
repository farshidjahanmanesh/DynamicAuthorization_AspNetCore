using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityLearning.Infrastructure
{
    /// <summary>
    /// this attribute jost work for IFormFile property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CheckImageSizeAttribute : ValidationAttribute
    {
        
        private int MBSize { get; set; }
        public CheckImageSizeAttribute(int mbSize)
        {
            if (mbSize < 0)
                MBSize = 1;
            else
                this.MBSize = mbSize;
        }
        public override bool IsValid(object value)
        {
            if (object.ReferenceEquals(value, null))
                return true;
            if (value is IFormFile)
            {
                var pic = value as FormFile;
                var filelen = Math.Floor((double)pic.Length / (1024 * 1024));
                if (filelen <= MBSize)
                    return true;
                return false;
            }
            return false;
        }
    }
}
