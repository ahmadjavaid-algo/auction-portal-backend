using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Hubs;
using AuctionPortal.InfrastructureLayer.Interfaces;
using AuctionPortal.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionPortal.Workers
{
    public class AuctionStatusUpdater : BackgroundService
    {
        private readonly ILogger<AuctionStatusUpdater> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval;

        // Windows for lifecycle admin notifications
        private static readonly TimeSpan StartingSoonWindow = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan EndingSoonWindow = TimeSpan.FromMinutes(10);

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

                    var auctionInfra = scope.ServiceProvider.GetRequiredService<IAuctionInfrastructure>();
                    var auctionBidApp = scope.ServiceProvider.GetRequiredService<IAuctionBidApplication>();
                    var invAucApp = scope.ServiceProvider.GetRequiredService<IInventoryAuctionApplication>();
                    var auctionApp = scope.ServiceProvider.GetRequiredService<IAuctionApplication>();
                    var notifApp = scope.ServiceProvider.GetRequiredService<INotificationApplication>();
                    var adminNotifApp = scope.ServiceProvider.GetRequiredService<IAdminNotificationApplication>();
                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                    // 1) Recalculate auction statuses (running/ended/etc.)
                    var rows = await auctionInfra.RecalculateStatuses();
                    if (rows > 0)
                    {
                        _logger.LogInformation("AuctionStatusUpdater: updated {Rows} auctions at {Local}", rows, DateTime.Now);
                    }

                    // 2) Admin lifecycle notifications (independent of favorites)
                    await GenerateAdminLifecycleNotificationsAsync(
                        invAucApp, auctionApp, adminNotifApp, hub, stoppingToken);

                    // 3) Based on updated state + timebox, send auction-won / auction-lost (bidder) + admin sold/lost
                    await ProcessAuctionResultsAsync(
                        auctionBidApp, invAucApp, auctionApp, notifApp, adminNotifApp, hub, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AuctionStatusUpdater failed");
                }

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // ignore cancellation during delay
                }
            }

            _logger.LogInformation("AuctionStatusUpdater stopped");
        }

        /// <summary>
        /// Emits admin lifecycle notifications (starting-soon/started/ending-soon/ended)
        /// for all active lots based purely on timeboxes (no dependency on favorites).
        /// Uses AdminNotificationHelper for broadcast; rely on DB upsert/unique key for idempotency.
        /// </summary>
        private static async Task GenerateAdminLifecycleNotificationsAsync(
            IInventoryAuctionApplication invAucApp,
            IAuctionApplication auctionApp,
            IAdminNotificationApplication adminNotifApp,
            IHubContext<NotificationHub> hub,
            CancellationToken ct)
        {
            var nowUtc = DateTime.UtcNow;

            // Expect IInventoryAuctionApplication to support GetList; pass Active=true to limit scope.
            var lots = await invAucApp.GetList(new InventoryAuction { Active = true });
            if (lots == null || lots.Count == 0) return;

            foreach (var lot in lots)
            {
                if (ct.IsCancellationRequested) return;

                var tb = await auctionApp.GetTimebox(new Auction { AuctionId = lot.AuctionId });
                if (tb == null || tb.StartEpochMsUtc <= 0 || tb.EndEpochMsUtc <= 0) continue;

                var startUtc = DateTimeOffset.FromUnixTimeMilliseconds(tb.StartEpochMsUtc).UtcDateTime;
                var endUtc = DateTimeOffset.FromUnixTimeMilliseconds(tb.EndEpochMsUtc).UtcDateTime;

                var auctionName = tb.AuctionName ?? "Auction";
                var titleBase = $"{auctionName} — Lot #{lot.InventoryId}";

                async Task UpsertAdminAsync(string type, string message)
                {
                    await AdminNotificationHelper.CreateAndBroadcastAsync(
                        adminNotifApp,
                        hub,
                        type: type,
                        title: $"{titleBase}",
                        message: message,
                        affectedUserId: null,
                        auctionId: lot.AuctionId,
                        inventoryAuctionId: lot.InventoryAuctionId);
                }

                // starting soon
                if (nowUtc >= startUtc - StartingSoonWindow && nowUtc < startUtc)
                {
                    await UpsertAdminAsync("auction-starting-soon", $"{titleBase} will start soon.");
                }

                // started
                if (nowUtc >= startUtc && nowUtc < startUtc.AddMinutes(5))
                {
                    await UpsertAdminAsync("auction-started", $"{titleBase} auction has started.");
                }

                // ending soon
                if (nowUtc >= endUtc - EndingSoonWindow && nowUtc < endUtc)
                {
                    await UpsertAdminAsync("auction-ending-soon", $"{titleBase} will end soon.");
                }

                // ended
                if (nowUtc >= endUtc)
                {
                    await UpsertAdminAsync("auction-ended", $"{titleBase} auction has ended.");
                }
            }
        }

        /// <summary>
        /// Finds ended lots based on auction timebox and sends:
        ///   - "auction-won" to the winning bidder
        ///   - "auction-lost" to other bidders
        ///   - and admin notifications "inventory-sold"/"inventory-lost"
        /// Uses dedupe via NotificationApplication to avoid duplicates to bidders.
        /// </summary>
        private static async Task ProcessAuctionResultsAsync(
    IAuctionBidApplication auctionBidApp,
    IInventoryAuctionApplication invAucApp,
    IAuctionApplication auctionApp,
    INotificationApplication notifApp,
    IAdminNotificationApplication adminNotifApp,
    IHubContext<NotificationHub> hub,
    CancellationToken ct)
        {
            var nowUtc = DateTime.UtcNow;

            // 1. Get ALL active inventory auctions (lots)
            var activeLots = await invAucApp.GetList(new InventoryAuction { Active = true });
            if (activeLots?.Any() != true) return;

            foreach (var lot in activeLots)
            {
                if (ct.IsCancellationRequested) return;

                var timebox = await auctionApp.GetTimebox(new Auction { AuctionId = lot.AuctionId });
                if (timebox?.EndEpochMsUtc <= 0) continue;

                var endUtc = DateTimeOffset.FromUnixTimeMilliseconds(timebox.EndEpochMsUtc).UtcDateTime;
                if (nowUtc < endUtc) continue; // not ended yet

                var auctionName = timebox.AuctionName ?? "Auction";
                var titleBase = $"{auctionName} — Lot #{lot.InventoryId}";

                // 2. Get all bids for this lot (active or not — winner might be inactive now)
                var allBidsForLot = await auctionBidApp.GetList(new AuctionBid
                {
                    InventoryAuctionId = lot.InventoryAuctionId
                });

                var winningBid = allBidsForLot
                    .Where(b => b.AuctionBidStatusName?.Equals("Won", StringComparison.OrdinalIgnoreCase) == true)
                    .OrderByDescending(b => b.CreatedDate ?? DateTime.MinValue)
                    .ThenByDescending(b => b.AuctionBidId)
                    .FirstOrDefault()
                    ?? allBidsForLot
                        .OrderByDescending(b => b.BidAmount)
                        .ThenByDescending(b => b.CreatedDate ?? DateTime.MinValue)
                        .ThenByDescending(b => b.AuctionBidId)
                        .FirstOrDefault();

                bool hasBids = allBidsForLot.Any();
                bool hasWinner = winningBid != null && winningBid.CreatedById.HasValue;

                var adminType = hasWinner ? "inventory-sold" : "inventory-lost";
                var adminTitle = hasWinner ? $"{titleBase} — Sold" : $"{titleBase} — No sale";
                var adminMessage = hasWinner
                    ? $"Lot sold: {titleBase}. Winner: User #{winningBid.CreatedById}, amount: {winningBid.BidAmount:N2}."
                    : hasBids
                        ? $"Lot ended without sale: {titleBase}. Reserve not met or no winning bid."
                        : $"Lot ended with no bids: {titleBase}.";

                // Broadcast admin notification (idempotent via helper)
                await AdminNotificationHelper.CreateAndBroadcastAsync(
                    adminNotifApp,
                    hub,
                    type: adminType,
                    title: adminTitle,
                    message: adminMessage,
                    affectedUserId: hasWinner ? winningBid.CreatedById : null,
                    auctionId: lot.AuctionId,
                    inventoryAuctionId: lot.InventoryAuctionId);

                // 3. Now send user notifications only if there are bids
                if (!hasBids) continue;

                var winnerUserId = hasWinner ? winningBid.CreatedById.Value : (int?)null;
                var bidderUserIds = allBidsForLot
                    .Where(b => b.CreatedById.HasValue)
                    .Select(b => b.CreatedById.Value)
                    .Distinct()
                    .ToList();

                foreach (var bidderId in bidderUserIds)
                {
                    if (ct.IsCancellationRequested) return;

                    bool isWinner = winnerUserId.HasValue && bidderId == winnerUserId;
                    await SendResultNotificationAsync(
                        notifApp, adminNotifApp, hub,
                        userId: bidderId,
                        auctionId: lot.AuctionId,
                        inventoryAuctionId: lot.InventoryAuctionId,
                        titleBase: titleBase,
                        isWinner: isWinner,
                        winningAmount: winningBid?.BidAmount ?? 0m,
                        ct: ct);
                }
            }
        }
        /// <summary>
        /// Dedupe + create + push 'auction-won' / 'auction-lost' notification for a user,
        /// and creates/broadcasts admin notification ("inventory-sold"/"inventory-lost").
        /// </summary>
        private static async Task SendResultNotificationAsync(
            INotificationApplication notifApp,
            IAdminNotificationApplication adminNotifApp,
            IHubContext<NotificationHub> hub,
            int userId,
            int auctionId,
            int inventoryAuctionId,
            string titleBase,
            bool isWinner,
            decimal winningAmount,
            CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return;

            var type = isWinner ? "auction-won" : "auction-lost";

            var existing = await notifApp.GetForUser(userId, unreadOnly: false, top: 200);
            var alreadyHas = existing.Any(n =>
                string.Equals(n.Type, type, StringComparison.OrdinalIgnoreCase) &&
                n.AuctionId == auctionId &&
                n.InventoryAuctionId == inventoryAuctionId);

            if (alreadyHas)
                return;

            string title = isWinner ? $"{titleBase} — You won" : $"{titleBase} — Auction ended";
            string message = isWinner
                ? $"Congratulations! You have won {titleBase} with a final bid of {winningAmount:N2}."
                : $"The auction for {titleBase} has ended. Your bid was not the winning bid (final winning amount: {winningAmount:N2}).";

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

            await hub.Clients
                .User(userId.ToString())
                .SendAsync("NotificationCreated", notification);

            var adminType = isWinner ? "inventory-sold" : "inventory-lost";
            var adminTitle = isWinner ? $"{titleBase} — Sold" : $"{titleBase} — Auction ended";
            var adminMessage = isWinner
                ? $"Lot sold: {titleBase}. Winning bidder #{userId}, amount {winningAmount:N2}."
                : $"Lot ended: {titleBase}. Bidder #{userId} did not win (final {winningAmount:N2}).";

            await AdminNotificationHelper.CreateAndBroadcastAsync(
                adminNotifApp,
                hub,
                type: adminType,
                title: adminTitle,
                message: adminMessage,
                affectedUserId: userId,
                auctionId: auctionId,
                inventoryAuctionId: inventoryAuctionId);
        }
    }
}
