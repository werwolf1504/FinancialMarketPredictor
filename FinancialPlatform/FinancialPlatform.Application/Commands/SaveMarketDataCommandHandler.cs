using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Application.Interfaces;
using FinancialPlatform.Domain.Entities;

using MediatR;

using MongoDB.Bson;

namespace FinancialPlatform.Application.Commands;

public record SaveMarketDataCommand(MarketTick MarketTick) : IRequest;

public class SaveMarketDataCommandHandler : IRequestHandler<SaveMarketDataCommand>
{
    private readonly IMarketDataRepository _marketDataRepository;

    public SaveMarketDataCommandHandler(IMarketDataRepository marketDataRepository)
    {
        _marketDataRepository = marketDataRepository;
    }

    public async Task Handle(SaveMarketDataCommand request, CancellationToken cancellationToken)
    {
        request.MarketTick.Id = ObjectId.GenerateNewId().ToString();

        await _marketDataRepository.InsertASync(request.MarketTick);
    }
}
