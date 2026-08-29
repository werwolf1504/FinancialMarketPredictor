using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Application.Interfaces;
using FinancialPlatform.Application.Queries;
using FinancialPlatform.Domain.Entities;

using FluentAssertions;

using Moq;

namespace FinancialPlatform.Application.Tests.Handlers;

public class GetLatestPriceQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnMarketTick_WhenTickerIsValid()
    {
        // Arrange
        var ticker = "AAPL";
        var expectedMarketTick = new MarketTick
        {
            Ticker = ticker,
            Price = 150.00m
        };

        var mockMarketDataProvider = new Mock<IMarketDataProvider>();
        mockMarketDataProvider.Setup(m => m.GetCurrentPriceAsync(ticker))
            .ReturnsAsync(expectedMarketTick);

        var handler = new GetLatestPriceQueryHandler(mockMarketDataProvider.Object);
        var query = new GetLatestPriceQuery(ticker);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Ticker.Should().Be(ticker);
        result.Price.Should().Be(expectedMarketTick.Price);
    }
}
