using IdentityLearning.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using UAParser;

namespace IdentityLearning.Infrastructure
{
    public static class DeviceCheckerMiddlewareExtensions
    {
        public static IApplicationBuilder UseDeviceChecker(
         this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DeviceCheckerMiddleware>();
        }
    }

    public class DeviceCheckerMiddleware
    {
        private readonly RequestDelegate next;
        public static DeviceGraphData Data { get; } = new DeviceGraphData();
        public DeviceCheckerMiddleware(RequestDelegate _next)
        {
            next = _next;
        }

        public static void Clear()
        {
            Data.Android = 0;
            Data.Desktop = 0;
            Data.IOS = 0;
            Data.Other = 0;
        }



        public static string GetUserOS(string userAgent)
        {
            // get a parser with the embedded regex patterns
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);
            return c.OS.Family;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection?.RemoteIpAddress.ToString();
            if (!CheckIpMiddleware.IsIpRequestToday(ip))
            {
                try
                {
                    var os = GetUserOS(context.Request?.Headers["User-Agent"]);
                    switch (os.ToLower())
                    {
                        case "android":
                            Data.Android++;
                            break;

                        case "windows":
                            Data.Desktop++;
                            break;


                        case "linux":
                            Data.Desktop++;
                            break;


                        case "ios":
                            Data.IOS++;
                            break;

                        default:
                            Data.Other++;
                            break;

                    }
                }
                catch (Exception)
                {

                }
            }

            await next(context);
        }
    }

}
