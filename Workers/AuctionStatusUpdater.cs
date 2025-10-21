using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AuctionPortal.InfrastructureLayer.Interfaces;

namespace AuctionPortal.Workers
{

    public class AuctionStatusUpdater : BackgroundService
    {
        private readonly ILogger<AuctionStatusUpdater> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval;

        public AuctionStatusUpdater(
            ILogger<AuctionStatusUpdater> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration config)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            var seconds = Math.Max(5, config.GetValue<int?>("AuctionStatus:IntervalSeconds") ?? 60);
            _interval = TimeSpan.FromSeconds(seconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuctionStatusUpdater started (interval: {Seconds}s)", _interval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var infra = scope.ServiceProvider.GetRequiredService<IAuctionInfrastructure>();

                  
                    var rows = await infra.RecalculateStatuses();
                    if (rows > 0)
                        _logger.LogInformation("AuctionStatusUpdater: updated {Rows} auctions at {Local}", rows, DateTime.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AuctionStatusUpdater failed");
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (TaskCanceledException) {  }
            }

            _logger.LogInformation("AuctionStatusUpdater stopped");
        }
    }
}
