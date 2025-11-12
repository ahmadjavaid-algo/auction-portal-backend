using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Hubs;
using AuctionPortal.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuctionPortal.Workers
{
    public class FavouriteAlertsWorker : BackgroundService
    {
        private readonly ILogger<FavouriteAlertsWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval;

        // tweak windows as desired
        private static readonly TimeSpan StartingSoonWindow = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan EndingSoonWindow = TimeSpan.FromMinutes(10);

        public FavouriteAlertsWorker(
            ILogger<FavouriteAlertsWorker> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration config)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            var seconds = Math.Max(10, config.GetValue<int?>("FavouriteAlerts:IntervalSeconds") ?? 60);
            _interval = TimeSpan.FromSeconds(seconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FavouriteAlertsWorker started (interval {Seconds}s)", _interval.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var favApp = scope.ServiceProvider.GetRequiredService<IFavouriteApplication>();
                    var invAucApp = scope.ServiceProvider.GetRequiredService<IInventoryAuctionApplication>();
                    var auctionApp = scope.ServiceProvider.GetRequiredService<IAuctionApplication>();
                    var notifApp = scope.ServiceProvider.GetRequiredService<INotificationApplication>();
                    var adminNotifApp = scope.ServiceProvider.GetRequiredService<IAdminNotificationApplication>(); // NEW
                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                    await ProcessAsync(favApp, invAucApp, auctionApp, notifApp, adminNotifApp, hub, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "FavouriteAlertsWorker error");
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (TaskCanceledException) { }
            }

            _logger.LogInformation("FavouriteAlertsWorker stopped");
        }

        private async Task ProcessAsync(
            IFavouriteApplication favApp,
            IInventoryAuctionApplication invAucApp,
            IAuctionApplication auctionApp,
            INotificationApplication notifApp,
            IAdminNotificationApplication adminNotifApp, // NEW
            IHubContext<NotificationHub> hub,
            CancellationToken ct)
        {
            var favourites = await favApp.GetList(new Favourite());
            var activeFavs = favourites.Where(f => f.Active).ToList();

            if (!activeFavs.Any())
                return;

            var nowUtc = DateTime.UtcNow;

            // group by user so we only load notifications per user once
            var favsByUser = activeFavs.GroupBy(f => f.UserId);

            foreach (var userGroup in favsByUser)
            {
                if (ct.IsCancellationRequested) return;

                var userId = userGroup.Key;

                // load recent notifications for dedupe
                var existing = await notifApp.GetForUser(userId, unreadOnly: false, top: 200);

                foreach (var fav in userGroup)
                {
                    if (ct.IsCancellationRequested) return;

                    // load inventory-auction (lot)
                    var invAuc = await invAucApp.Get(new InventoryAuction
                    {
                        InventoryAuctionId = fav.InventoryAuctionId
                    });

                    if (invAuc == null)
                        continue;

                    var timebox = await auctionApp.GetTimebox(new Auction
                    {
                        AuctionId = invAuc.AuctionId
                    });

                    if (timebox == null ||
                        timebox.StartEpochMsUtc <= 0 ||
                        timebox.EndEpochMsUtc <= 0)
                        continue;

                    var startUtc = DateTimeOffset.FromUnixTimeMilliseconds(timebox.StartEpochMsUtc).UtcDateTime;
                    var endUtc = DateTimeOffset.FromUnixTimeMilliseconds(timebox.EndEpochMsUtc).UtcDateTime;

                    var auctionName = timebox.AuctionName ?? "Auction";
                    var titleBase = $"{auctionName} — Lot #{invAuc.InventoryId}";

                    bool Has(string type) =>
                        existing.Any(n =>
                            string.Equals(n.Type, type, StringComparison.OrdinalIgnoreCase) &&
                            n.AuctionId == invAuc.AuctionId &&
                            n.InventoryAuctionId == fav.InventoryAuctionId);

                    // --- auction-starting-soon ---
                    if (!Has("auction-starting-soon") &&
                        nowUtc >= startUtc - StartingSoonWindow &&
                        nowUtc < startUtc)
                    {
                        await CreateAndPushAsync(
                            notifApp, adminNotifApp, hub, userId,
                            type: "auction-starting-soon",
                            title: $"{titleBase} starting soon",
                            message: $"{titleBase} will start soon.",
                            auctionId: invAuc.AuctionId,
                            inventoryAuctionId: fav.InventoryAuctionId);
                    }

                    // --- auction-started ---
                    if (!Has("auction-started") &&
                        nowUtc >= startUtc &&
                        nowUtc < startUtc.AddMinutes(5))
                    {
                        await CreateAndPushAsync(
                            notifApp, adminNotifApp, hub, userId,
                            type: "auction-started",
                            title: $"{titleBase} is now live",
                            message: $"{titleBase} auction has started.",
                            auctionId: invAuc.AuctionId,
                            inventoryAuctionId: fav.InventoryAuctionId);
                    }

                    // --- auction-ending-soon ---
                    if (!Has("auction-ending-soon") &&
                        nowUtc >= endUtc - EndingSoonWindow &&
                        nowUtc < endUtc)
                    {
                        await CreateAndPushAsync(
                            notifApp, adminNotifApp, hub, userId,
                            type: "auction-ending-soon",
                            title: $"{titleBase} ending soon",
                            message: $"{titleBase} will end soon. Place your final bids.",
                            auctionId: invAuc.AuctionId,
                            inventoryAuctionId: fav.InventoryAuctionId);
                    }

                    // --- auction-ended ---
                    if (!Has("auction-ended") &&
                        nowUtc >= endUtc)
                    {
                        await CreateAndPushAsync(
                            notifApp, adminNotifApp, hub, userId,
                            type: "auction-ended",
                            title: $"{titleBase} ended",
                            message: $"{titleBase} auction has ended.",
                            auctionId: invAuc.AuctionId,
                            inventoryAuctionId: fav.InventoryAuctionId);
                    }
                }
            }
        }

        private static async Task CreateAndPushAsync(
            INotificationApplication notifApp,
            IAdminNotificationApplication adminNotifApp,          // NEW
            IHubContext<NotificationHub> hub,
            int userId,
            string type,
            string title,
            string message,
            int auctionId,
            int inventoryAuctionId)
        {
            // Bidder notification
            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedById = userId,
                AuctionId = auctionId,
                InventoryAuctionId = inventoryAuctionId
            };

            await notifApp.Add(notification);

            // Generic SignalR event for bidder client
            await hub.Clients
                .User(userId.ToString())
                .SendAsync("NotificationCreated", notification);

            // Admin global notification
            await AdminNotificationHelper.CreateAndBroadcastAsync(
                adminNotifApp,
                hub,
                type: type,                     // reuse same type
                title: title,
                message: message,
                affectedUserId: userId,
                auctionId: auctionId,
                inventoryAuctionId: inventoryAuctionId);
        }
    }
}
