using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using FinancialPlatform.Application.Interfaces;
using FinancialPlatform.Domain.Entities;

using Microsoft.Extensions.Caching.Distributed;

namespace FinancialPlatform.Infrastructure.Clients;

public class CachedMarketDataProvider : IMarketDataProvider
{
    private readonly IMarketDataProvider _marketDataProvider;
    private readonly IDistributedCache _distributedCache;

    public CachedMarketDataProvider(IMarketDataProvider marketDataProvider, IDistributedCache distributedCache)
    {
        _marketDataProvider = marketDataProvider;
        _distributedCache = distributedCache;
    }

    public async Task<MarketTick> GetCurrentPriceAsync(string ticker)
    {
        string cacheKey = $"price_{ticker}";
        // Implementation for getting cached price

        var cachedData = await _distributedCache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            // Deserialize cached data and return
             return JsonSerializer.Deserialize<MarketTick>(cachedData);
        }

        // If not in cache, fetch from the actual market data provider
        var marketTick = await _marketDataProvider.GetCurrentPriceAsync(ticker);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache for 5 minutes
        };

        // Cache the result
        await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(marketTick), options);
        return marketTick;
    }
}
