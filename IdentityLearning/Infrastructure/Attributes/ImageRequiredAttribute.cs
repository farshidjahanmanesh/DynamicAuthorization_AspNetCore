using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ImageRequiredAttribute:ValidationAttribute
    {
        public const int ImageMinimumBytes = 512;

        public override bool IsValid(object value)
        {
            if (object.ReferenceEquals(value, null))
                return true;

            if(value is IFormFile)
            {
                var file = value as IFormFile;
                return GetImageFormat(ObjectToByteArray(file).Result);
            }
            return false;
        }
        public static bool GetImageFormat(byte[] bytes)
        {
            var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
            var gif = Encoding.ASCII.GetBytes("GIF");    // GIF
            var png = new byte[] { 137, 80, 78, 71 };    // PNG
            var tiff = new byte[] { 73, 73, 42 };         // TIFF
            var tiff2 = new byte[] { 77, 77, 42 };         // TIFF
            var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
                return true;

            if (gif.SequenceEqual(bytes.Take(gif.Length)))
                return true;

            if (png.SequenceEqual(bytes.Take(png.Length)))
                return true;

            if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
                return true;

            if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
                return true;

            if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                return true;

            if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                return true;

            return false;
        }

        private static async Task<byte[]> ObjectToByteArray(IFormFile postedFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await postedFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

       

    }
}

