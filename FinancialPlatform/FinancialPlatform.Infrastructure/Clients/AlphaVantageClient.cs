using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

using FinancialPlatform.Application.Interfaces;
using FinancialPlatform.Domain.Entities;
using FinancialPlatform.Infrastructure.DTO;
using FinancialPlatform.Infrastructure.Settings;

using Microsoft.Extensions.Options;

namespace FinancialPlatform.Infrastructure.Clients;

public class AlphaVantageClient : IMarketDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AlphaVantageClient(HttpClient httpClient, IOptions<ExternalApiSettings> options)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.AlphaVantageApiKey;
        _httpClient.BaseAddress = new Uri("https://www.alphavantage.co/query");
    }

    public async Task<MarketTick> GetCurrentPriceAsync(string ticker)
    {
        var url = $"?function=GLOBAL_QUOTE&symbol={ticker}&apikey={_apiKey}";

        var respone = await _httpClient.GetFromJsonAsync<AlphaVantageQouteDTO>(url);

        if (respone == null || respone.GlobalQuote == null)
        {
            throw new Exception($"Failed to retrieve data for ticker: {ticker}");
        }

        return new MarketTick
        {
            Ticker = ticker,
            Price = respone.GlobalQuote.Price,
            Timestamp = respone.GlobalQuote.LatestTradingDay
        };
    }
}
