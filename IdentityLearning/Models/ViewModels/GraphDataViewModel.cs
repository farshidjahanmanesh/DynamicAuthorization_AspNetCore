using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityLearning.Models.ViewModels
{
    public class GraphDataViewModel
    {
        private readonly List<Claim> claims;
        private readonly TestIdentityDbContext ctx;

        public GraphDataViewModel(List<Claim> claims, TestIdentityDbContext _ctx)
        {
            this.claims = claims;
            ctx = _ctx;
            StartSetDatas();
        }
        public string ViewerVisitorGraphSerialize { get; set; }
        public string DeviceCheckerSerialize { get; set; }
        public string BrowserVisitorSerialize { get; set; }
        private void StartSetDatas()
        {
            SetViewerVisitorData();
            SetDeviceCheckerData();
            SetBrowserVisitorData();
        }

        private void SetViewerVisitorData()
        {
            if (claims.Any(c => c.Type == "AccessLevel" && c.Value == graphPermission.ViewerVisitor.ToString()))
            {
                var result = ctx.ViewerCounter.Where(c => c.Date >= DateTime.Now.AddDays(-7))
                    .ToList();
                ViewerVisitorGraphData serialObject = new ViewerVisitorGraphData();
                foreach (var item in result)
                {
                    serialObject.Counts.Add(item.Count);
                    serialObject.Dates.Add(item.Date.ToShortDateString());
                }
                ViewerVisitorGraphSerialize = JsonConvert.SerializeObject(serialObject);
            }
        }

        private void SetDeviceCheckerData()
        {
            if (claims.Any(c => c.Type == "AccessLevel" && c.Value == graphPermission.DeviceChecker.ToString()))
            {
                var result = ctx.DeviceCounter.Where(c => c.Date > DateTime.Now.AddDays(-30)).ToList();
                int totalCount = 0, tAndorid = 0, tIOS = 0, tDesktop = 0, tOther = 0;
                tAndorid = result.Sum(c => c.Android);
                tIOS = result.Sum(c => c.IOS);
                tDesktop = result.Sum(c => c.Desktop);
                tOther = result.Sum(c => c.Other);
                totalCount = tAndorid + tIOS + tDesktop + tOther;

                DeviceGraphData mb = new DeviceGraphData();

                mb.Android =(int) (((double)tAndorid / totalCount) * 100);
                mb.IOS = (int)(((double)tIOS / totalCount) * 100);
                mb.Desktop = (int)(((double)tDesktop / totalCount) * 100);
                mb.Other = (int)(((double)tOther / totalCount) * 100);

                DeviceCheckerSerialize = JsonConvert.SerializeObject(mb);
            }

        }

        private void SetBrowserVisitorData()
        {
            if (claims.Any(c => c.Type == "AccessLevel" && c.Value == graphPermission.BrowserVisitor.ToString()))
            {
                var result = ctx.BrowserCounter.Where(c => c.Date > DateTime.Now.AddDays(-30)).ToList();
                int totalCount = 0, firefox = 0, safari = 0, chorome = 0, edge = 0, ie = 0, other = 0;

                var bg = new BrowserGraphData();
                result.ForEach(c => {
                    firefox += c.FireFox;
                    safari += c.Safari;
                    chorome += c.Chorome;
                    edge += c.Edge;
                    ie += c.IE;
                    other += c.Other;
                });
                totalCount = firefox + safari + chorome + edge + ie + other;
                bg.FireFox =(int) (((double)firefox / totalCount) * 100);
                bg.Safari = (int)(((double)safari / totalCount) * 100);
                bg.Chorome = (int)(((double)chorome / totalCount) * 100);
                bg.Edge = (int)(((double)edge / totalCount) * 100);
                bg.IE = (int)(((double)ie / totalCount) * 100);
                bg.Other = (int)(((double)other / totalCount) * 100);

                BrowserVisitorSerialize = JsonConvert.SerializeObject(bg);

            }

        }
    }
}
