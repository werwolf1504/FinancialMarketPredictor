using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Application.Interfaces;
using FinancialPlatform.Domain.Entities;

using MediatR;

namespace FinancialPlatform.Application.Queries;

public record GetLatestPriceQuery(string Ticker) : IRequest<MarketTick>;

public class GetLatestPriceQueryHandler : IRequestHandler<GetLatestPriceQuery, MarketTick>
{
    private readonly IMarketDataProvider _marketDataProvider;

    public GetLatestPriceQueryHandler(IMarketDataProvider marketDataProvider)
    {
        _marketDataProvider = marketDataProvider;
    }

    public async Task<MarketTick> Handle(GetLatestPriceQuery request, CancellationToken cancellationToken)
    {
        return await _marketDataProvider.GetCurrentPriceAsync(request.Ticker);
    }
}
