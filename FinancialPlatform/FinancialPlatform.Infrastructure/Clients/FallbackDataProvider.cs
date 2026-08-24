using System;
using System.Collections.Generic;
using System.Text;

using DnsClient.Internal;

using FinancialPlatform.Application.Interfaces;
using FinancialPlatform.Domain.Entities;

using Microsoft.Extensions.Logging;

namespace FinancialPlatform.Infrastructure.Clients;

public class FallbackDataProvider : IMarketDataProvider
{
    private readonly FinnhubClient _finnhubClient;
    private readonly AlphaVantageClient _alphaVantageClient;
    private readonly ILogger<FallbackDataProvider> _logger;

    public FallbackDataProvider(FinnhubClient finnhubClient, AlphaVantageClient alphaVantageClient, ILogger<FallbackDataProvider> logger)
    {
        _finnhubClient = finnhubClient;
        _alphaVantageClient = alphaVantageClient;
        _logger = logger;
    }

    public async Task<MarketTick> GetCurrentPriceAsync(string ticker)
    {
        try
        {
            return await _finnhubClient.GetCurrentPriceAsync(ticker);
        }
        catch (Exception)
        {
            _logger.LogWarning("FinnhubClient failed, falling back to AlphaVantageClient for ticker: {Ticker}", ticker);
            return await _alphaVantageClient.GetCurrentPriceAsync(ticker);
        }
    }
}
