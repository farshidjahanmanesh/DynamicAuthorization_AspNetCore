using Newtonsoft.Json;
using SharedServices.Context;
using SharedServices.Models.IdentityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
namespace SharedServices.GraphModel
{
    public class GraphRepository
    {
        private List<Claim> _claims;
        private readonly TestIdentityDbContext _ctx;
        private int _dayFroVisitor = -7;
        private int _dayFroBrowser = -30;
        private int _dayFroDevice = -30;

        public string ViewerVisitorGraphSerialize { get; protected set; }
        public string DeviceCheckerSerialize { get; protected set; }
        public string BrowserVisitorSerialize { get; protected set; }

        public int DayFroVisitor
        {
            set
            {
                _dayFroVisitor = (value > 0) ? value * -1 : value;
            }
        }

        public int DayForBrowser
        {
            set
            {
                _dayFroBrowser = (value > 0) ? value * -1 : value;
            }
        }
        public int DayForDevice
        {
            set
            {
                _dayFroDevice = (value > 0) ? value * -1 : value;
            }
        }


        public GraphRepository(TestIdentityDbContext ctx)
        {
            _ctx = ctx;
        }
        public void StartGetData(IEnumerable<Claim> claims)
        {
            if (ReferenceEquals(claims, null))
                throw new Exception("claims are null");

            _claims = claims.ToList();
            SetSerializeData();
        }

        private void SetSerializeData()
        {
            SetSerializeVisitorData();
            SetSerializeDeviceData();
            SetSerializeBrowserData();
        }

        private void SetSerializeVisitorData()
        {
            if (_claims.Any(c => c.Type == "AccessLevel" && c.Value == PersmissionsEnum.ViewerVisitor.ToString()))
            {
                var resultVisitor = _ctx.ViewerCounter.Where(c => c.Date >= DateTime.Now.AddDays(_dayFroVisitor))
                    .ToList();
                ViewerVisitorGraph serialObject = new ViewerVisitorGraph();
                foreach (var item in resultVisitor)
                {
                    serialObject.Counts.Add(item.Count);
                    serialObject.Dates.Add(item.Date.ToShortDateString());
                }
                ViewerVisitorGraphSerialize = JsonConvert.SerializeObject(serialObject);
            }
        }

        private void SetSerializeDeviceData()
        {
            if (_claims.Any(c => c.Type == "AccessLevel" && c.Value == PersmissionsEnum.DeviceChecker.ToString()))
            {
                var result = _ctx.DeviceCounter.Where(c => c.Date > DateTime.Now.AddDays(_dayFroDevice)).ToList();
                int totalCount = 0, tAndorid = 0, tIOS = 0, tDesktop = 0, tOther = 0;
                totalCount += tAndorid = result.Sum(c => c.Android);
                totalCount += tIOS = result.Sum(c => c.IOS);
                totalCount += tDesktop = result.Sum(c => c.Desktop);
                totalCount += tOther = result.Sum(c => c.Other);
                //totalCount = tAndorid + tIOS + tDesktop + tOther;

                DeviceGraph mb = new DeviceGraph();

                mb.Android = (int)(((double)tAndorid / totalCount) * 100);
                mb.IOS = (int)(((double)tIOS / totalCount) * 100);
                mb.Desktop = (int)(((double)tDesktop / totalCount) * 100);
                mb.Other = (int)(((double)tOther / totalCount) * 100);

                DeviceCheckerSerialize = JsonConvert.SerializeObject(mb);
            }

        }

        private void SetSerializeBrowserData()
        {
            if (_claims.Any(c => c.Type == "AccessLevel" && c.Value == PersmissionsEnum.BrowserVisitor.ToString()))
            {
                var result = _ctx.BrowserCounter.Where(c => c.Date > DateTime.Now.AddDays(_dayFroBrowser)).ToList();
                int totalCount = 0, firefox = 0, safari = 0, chorome = 0, edge = 0, ie = 0, other = 0;

                var bg = new BrowserGraph();
                result.ForEach(c =>
                {
                    firefox += c.FireFox;
                    safari += c.Safari;
                    chorome += c.Chorome;
                    edge += c.Edge;
                    ie += c.IE;
                    other += c.Other;
                });
                totalCount = firefox + safari + chorome + edge + ie + other;
                bg.FireFox = (int)(((double)firefox / totalCount) * 100);
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
