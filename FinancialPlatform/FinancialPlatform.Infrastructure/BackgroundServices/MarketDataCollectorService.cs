using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Application.Commands;
using FinancialPlatform.Application.Queries;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinancialPlatform.Infrastructure.BackgroundServices;

public class MarketDataCollectorService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MarketDataCollectorService(ILogger<MarketDataCollectorService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string[] tickers = new[] { "AAPL", "MSFT", "TSLA" }; // Add your tickers here

        while (!stoppingToken.IsCancellationRequested) {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                foreach (var ticker in tickers)
                {
                    try
                    {
                        var priceQuery = new GetLatestPriceQuery(ticker);
                        var tick = await mediator.Send(priceQuery, stoppingToken);

                        var saveCommand = new SaveMarketDataCommand(tick);
                        await mediator.Send(saveCommand, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error collecting market data for ticker {Ticker}", ticker);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
