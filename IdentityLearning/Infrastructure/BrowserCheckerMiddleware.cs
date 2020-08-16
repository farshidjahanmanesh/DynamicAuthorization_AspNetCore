using IdentityLearning.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UAParser;

namespace IdentityLearning.Infrastructure
{

    public static class BrowserCheckerMiddlewareExtensions
    {
        public static IApplicationBuilder UseBrowserChecker(
         this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BrowserCheckerMiddleware>();
        }
    }
    public class BrowserCheckerMiddleware
    {
        private readonly RequestDelegate next;
        public static BrowserGraphData Data { get; } = new BrowserGraphData();
        public BrowserCheckerMiddleware(RequestDelegate _next)
        {
            next = _next;
        }
        public static void Clear()
        {
            Data.Edge = 0;
            Data.Chorome = 0;
            Data.FireFox = 0;
            Data.IE = 0;
            Data.Other = 0;
            Data.Safari = 0;
        }

        private static string GetUserBrowser(string userAgent)
        {
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);
            return c.UserAgent.Family;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection?.RemoteIpAddress.ToString();
            if (!CheckIpMiddleware.IsIpRequestToday(ip))
            {
                try
                {
                    var browser = GetUserBrowser(context.Request?.Headers["User-Agent"]);
                    switch (browser.ToLower())
                    {
                        case "chorome":
                            Data.Chorome++;
                            break;

                        case "edge":
                            Data.Edge++;
                            break;

                        case "firefox":
                            Data.FireFox++;
                            break;

                        case "ie":
                            Data.IE++;
                            break;


                        case "safari":
                            Data.Safari++;
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
