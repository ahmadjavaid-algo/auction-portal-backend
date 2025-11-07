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
                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                    // 1) Recalculate auction statuses (running/ended/etc.)
                    var rows = await auctionInfra.RecalculateStatuses();
                    if (rows > 0)
                    {
                        _logger.LogInformation("AuctionStatusUpdater: updated {Rows} auctions at {Local}", rows, DateTime.Now);
                    }

                    // 2) Based on updated state + timebox, send auction-won / auction-lost notifications
                    await ProcessAuctionResultsAsync(
                        auctionBidApp,
                        invAucApp,
                        auctionApp,
                        notifApp,
                        hub,
                        stoppingToken);
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
        /// Finds ended lots based on auction timebox and sends:
        ///   - "auction-won" to the winning bidder
        ///   - "auction-lost" to other bidders
        /// Uses dedupe via NotificationApplication to avoid duplicates.
        /// </summary>
        private static async Task ProcessAuctionResultsAsync(
            IAuctionBidApplication auctionBidApp,
            IInventoryAuctionApplication invAucApp,
            IAuctionApplication auctionApp,
            INotificationApplication notifApp,
            IHubContext<NotificationHub> hub,
            CancellationToken ct)
        {
            var nowUtc = DateTime.UtcNow;

            // Get all bids (trimmed; GetList should bring AuctionId, InventoryAuctionId, BidAmount, StatusName, CreatedById, CreatedDate, Active)
            var allBids = await auctionBidApp.GetList(new AuctionBid());
            var activeBids = allBids.Where(b => b.Active && b.InventoryAuctionId > 0).ToList();

            if (!activeBids.Any())
                return;

            // Group by lot (InventoryAuctionId)
            var lots = activeBids
                .GroupBy(b => b.InventoryAuctionId)
                .ToList();

            foreach (var lotGroup in lots)
            {
                if (ct.IsCancellationRequested) return;

                var sampleBid = lotGroup.First();
                var auctionId = sampleBid.AuctionId;
                var inventoryAuctionId = sampleBid.InventoryAuctionId;

                // Load auction timebox to know if it's ended
                var timebox = await auctionApp.GetTimebox(new Auction
                {
                    AuctionId = auctionId
                });

                if (timebox == null ||
                    timebox.EndEpochMsUtc <= 0)
                    continue;

                var endUtc = DateTimeOffset.FromUnixTimeMilliseconds(timebox.EndEpochMsUtc).UtcDateTime;
                if (nowUtc < endUtc)
                {
                    // auction not ended yet; skip this lot
                    continue;
                }

                // Load InventoryAuction for title (Lot #)
                var invAuc = await invAucApp.Get(new InventoryAuction
                {
                    InventoryAuctionId = inventoryAuctionId
                });

                if (invAuc == null)
                    continue;

                var auctionName = timebox.AuctionName ?? "Auction";
                var titleBase = $"{auctionName} — Lot #{invAuc.InventoryId}";

                // Determine winner:
                // 1) Prefer explicit "Won" status if your SP sets it
                var winningBid = lotGroup
                    .Where(b =>
                        string.Equals(b.AuctionBidStatusName ?? string.Empty,
                                      "Won",
                                      StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(b => b.CreatedDate ?? DateTime.MinValue)
                    .ThenByDescending(b => b.AuctionBidId)
                    .FirstOrDefault();

                // 2) Fallback: highest amount / latest bid
                if (winningBid == null)
                {
                    winningBid = lotGroup
                        .OrderByDescending(b => b.BidAmount)
                        .ThenByDescending(b => b.CreatedDate ?? DateTime.MinValue)
                        .ThenByDescending(b => b.AuctionBidId)
                        .FirstOrDefault();
                }

                if (winningBid == null || !winningBid.CreatedById.HasValue)
                    continue;

                var winnerUserId = winningBid.CreatedById.Value;

                // losers = all other users who placed at least one bid on this lot
                var losingUsers = lotGroup
                    .Where(b => b.CreatedById.HasValue && b.CreatedById.Value != winnerUserId)
                    .GroupBy(b => b.CreatedById!.Value)
                    .Select(g => new
                    {
                        UserId = g.Key,
                        // last bid from this user (mainly for message context if needed later)
                        LastBid = g
                            .OrderByDescending(b => b.CreatedDate ?? DateTime.MinValue)
                            .ThenByDescending(b => b.AuctionBidId)
                            .First()
                    })
                    .ToList();

                // 1) Notify winner: "auction-won"
                await SendResultNotificationAsync(
                    notifApp,
                    hub,
                    userId: winnerUserId,
                    auctionId: auctionId,
                    inventoryAuctionId: inventoryAuctionId,
                    titleBase: titleBase,
                    isWinner: true,
                    winningAmount: winningBid.BidAmount,
                    ct: ct);

                // 2) Notify losers: "auction-lost"
                foreach (var loser in losingUsers)
                {
                    if (ct.IsCancellationRequested) return;

                    await SendResultNotificationAsync(
                        notifApp,
                        hub,
                        userId: loser.UserId,
                        auctionId: auctionId,
                        inventoryAuctionId: inventoryAuctionId,
                        titleBase: titleBase,
                        isWinner: false,
                        winningAmount: winningBid.BidAmount,
                        ct: ct);
                }
            }
        }

        /// <summary>
        /// Dedupe + create + push 'auction-won' / 'auction-lost' notification for a user.
        /// </summary>
        private static async Task SendResultNotificationAsync(
            INotificationApplication notifApp,
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

            // Dedupe: check if we've already sent this type for this user + lot
            var existing = await notifApp.GetForUser(userId, unreadOnly: false, top: 200);
            var alreadyHas = existing.Any(n =>
                string.Equals(n.Type, type, StringComparison.OrdinalIgnoreCase) &&
                n.AuctionId == auctionId &&
                n.InventoryAuctionId == inventoryAuctionId);

            if (alreadyHas)
                return;

            string title;
            string message;

            if (isWinner)
            {
                title = $"{titleBase} — You won";
                message = $"Congratulations! You have won {titleBase} with a final bid of {winningAmount:N2}.";
            }
            else
            {
                title = $"{titleBase} — Auction ended";
                message = $"The auction for {titleBase} has ended. Your bid was not the winning bid (final winning amount: {winningAmount:N2}).";
            }

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
        }
    }
}
