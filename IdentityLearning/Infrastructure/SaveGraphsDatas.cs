using IdentityLearning.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using IdentityLearning.Models.Entities;
namespace IdentityLearning.Infrastructure
{
    public class SaveGraphsDatas : IDisposable
    {
        private readonly TestIdentityDbContext _ctx;

        public SaveGraphsDatas(TestIdentityDbContext _ctx)
        {
            this._ctx = _ctx;
        }

        public void Dispose()
        {
            _ctx.Dispose();
        }

        public void Save()
        {
            _ctx.ViewerCounter.Add(new ViewerCounter()
            {
                Date = DateTime.Now,
                Count = CheckIpMiddleware.CounterIp
            });

            _ctx.BrowserCounter.Add(new BrowserCounter()
            {
                Date=DateTime.Now,
                Chorome=BrowserCheckerMiddleware.Data.Chorome,
                Edge=BrowserCheckerMiddleware.Data.Edge,
                FireFox=BrowserCheckerMiddleware.Data.FireFox,
                IE=BrowserCheckerMiddleware.Data.IE,
                Safari=BrowserCheckerMiddleware.Data.Safari,
                Other=BrowserCheckerMiddleware.Data.Other,
            });

            _ctx.DeviceCounter.Add(new DeviceCounter()
            {
                Date = DateTime.Now,
                Desktop = DeviceCheckerMiddleware.Data.Desktop,
                IOS = DeviceCheckerMiddleware.Data.IOS,
                Android = DeviceCheckerMiddleware.Data.Android,
                Other = DeviceCheckerMiddleware.Data.Other,
            });

            CheckIpMiddleware.Clear();
            DeviceCheckerMiddleware.Clear();
            BrowserCheckerMiddleware.Clear();

            _ctx.SaveChanges();
            
        }
    }

}
