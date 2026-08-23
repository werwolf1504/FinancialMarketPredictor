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

public class FinnhubClient : IMarketDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public FinnhubClient(HttpClient httpClient, IOptions<ExternalApiSettings> options)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.FinnhubApiKey;

        _httpClient.BaseAddress = new Uri("https://finnhub.io/api/v1/");
    }

    public async Task<MarketTick> GetCurrentPriceAsync(string ticker)
    {
        var url = $"quote?symbol={ticker}&token={_apiKey}";

        var response = await _httpClient.GetFromJsonAsync<FinnhubQouteDTO>(url);

        if(response == null)
        {
            throw new Exception($"Failed to retrieve data for ticker: {ticker}");
        }

        return new MarketTick { 
            Ticker = ticker,
            Price = response.CurrentPrice, 
            Timestamp = DateTimeOffset.FromUnixTimeSeconds(response.Timestamp).UtcDateTime 
        };
    }
}
