using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedServices.Context;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace IdentityLearning.Infrastructure
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly object balanceLock = new object();
        private Timer _timer;
        private Timer _keepAlive;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IConfiguration configuration;

        public TimedHostedService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            this.scopeFactory = scopeFactory;
            this.configuration = configuration;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(
                    Convert.ToInt32(configuration["DataGraphTime"])
                    ));

            _keepAlive = new Timer(KeepAlive, null, TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(
                    Convert.ToInt32(configuration["KeepAliveTime"])));

            return Task.CompletedTask;
        }
        private async void KeepAlive(object state)
        {
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetAsync(configuration["WebAddress"]);


        }
        private void DoWork(object state)
        {
            lock (balanceLock)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetService<TestIdentityDbContext>();
                    using (var saveDatas = new SaveGraphsDatas(db))
                    {
                        saveDatas.Save();
                    }
                }
            }


        }

        public Task StopAsync(CancellationToken stoppingToken)
        {

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}
