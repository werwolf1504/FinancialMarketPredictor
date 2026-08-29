using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Application.Commands;
using FinancialPlatform.Application.Interfaces;
using FinancialPlatform.Contracts;
using FinancialPlatform.Domain.Entities;

using MassTransit;

using Moq;

namespace FinancialPlatform.Application.Tests.Handlers;

public class SaveMarketDataCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldSaveMarketTick_WhenTickerIsValid()
    {
        // Arrange
        var mockRepository = new Mock<IMarketDataRepository>();
        var mockPublishEndpoint = new Mock<IPublishEndpoint>();

        var tickToSave = new MarketTick
        {
            Ticker = "NVDA",
            Price = 450.00m,
            Timestamp = DateTime.UtcNow
        };

        var command = new SaveMarketDataCommand(tickToSave);

        var handler = new SaveMarketDataCommandHandler(mockRepository.Object, mockPublishEndpoint.Object);

        await handler.Handle(command, CancellationToken.None);

        mockRepository.Verify(repo => repo.InsertASync(It.Is<MarketTick>(mt => mt.Ticker == "NVDA")), Times.Once);

        mockPublishEndpoint.Verify(pub => pub.Publish(It.Is<MarketDataCollectedEvent>(mt => mt.Ticker == "NVDA" && mt.Price == 450.00m), It.IsAny<CancellationToken>()), Times.Once);
    }
}
