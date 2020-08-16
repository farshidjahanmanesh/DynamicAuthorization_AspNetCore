using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityLearning.Infrastructure
{
   

    public static class CheckIpMiddlewareExtensions
    {
        public static IApplicationBuilder UseCheckIp(
          this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckIpMiddleware>();
        }
    }
    public class CheckIpMiddleware
    {
        private readonly RequestDelegate next;
        private static HashSet<string> _Ips = new HashSet<string>();
        public static int CounterIp => _Ips.Count;
        public static bool IsIpRequestToday(string ip)
        {
            return _Ips.Contains(ip);
        }
        public static void Clear()
        {
            _Ips.Clear();
        }
        public CheckIpMiddleware(RequestDelegate _next)
        {
            next = _next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var ip = context.Connection?.RemoteIpAddress.ToString();
                if (ip != null)
                    _Ips.Add(ip);
            }
            catch (Exception)
            {
            }



            await next(context);
        }
    }
}
