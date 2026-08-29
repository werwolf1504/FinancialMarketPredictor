using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Application.Interfaces;
using FinancialPlatform.Contracts;
using FinancialPlatform.Domain.Entities;

using MassTransit;

using MediatR;

using MongoDB.Bson;

namespace FinancialPlatform.Application.Commands;

public record SaveMarketDataCommand(MarketTick MarketTick) : IRequest;

public class SaveMarketDataCommandHandler : IRequestHandler<SaveMarketDataCommand>
{
    private readonly IMarketDataRepository _marketDataRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public SaveMarketDataCommandHandler(IMarketDataRepository marketDataRepository, IPublishEndpoint publishEndpoint)
    {
        _marketDataRepository = marketDataRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(SaveMarketDataCommand request, CancellationToken cancellationToken)
    {
        request.MarketTick.Id = ObjectId.GenerateNewId().ToString();

        await _marketDataRepository.InsertASync(request.MarketTick);

        var marketData = new MarketDataCollectedEvent(request.MarketTick.Ticker, request.MarketTick.Price, request.MarketTick.Timestamp);

        await _publishEndpoint.Publish(marketData, cancellationToken);
    }
}
