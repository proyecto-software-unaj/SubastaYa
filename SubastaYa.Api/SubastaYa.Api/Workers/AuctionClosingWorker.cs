
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace SubastaYa.Api.Workers
{
    public class AuctionClosingWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuctionClosingWorker> _logger;

        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(15);

        public AuctionClosingWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<AuctionClosingWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    using var scope = _scopeFactory.CreateScope();
                    var closingService = scope.ServiceProvider.GetRequiredService<AuctionClosingService>();
                
                    var activated = await closingService.ActivateScheduledAuctionsAsync(stoppingToken);
                    if (activated > 0)
                    {
                        _logger.LogInformation("{Count} subasta(s) activada(s) por el worker.", activated);
                    }
                
                    var closed = await closingService.CloseExpiredAuctionsAsync(stoppingToken);
                    if (closed > 0)
                    {
                        _logger.LogInformation("{Count} subasta(s) cerrada(s) por el worker.", closed);
                    }

                }
                catch (Exception ex)
                {
                    
                    _logger.LogError(ex, "Error al procesar subastas vencidas.");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }
    }
}
