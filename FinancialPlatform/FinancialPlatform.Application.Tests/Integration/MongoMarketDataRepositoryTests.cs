using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Domain.Entities;
using FinancialPlatform.Infrastructure.Data;
using FinancialPlatform.Infrastructure.Settings;

using FluentAssertions;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

using Testcontainers.MongoDb;

namespace FinancialPlatform.Application.Tests.Integration;

public class MongoMarketDataRepositoryTests : IAsyncLifetime
{
    private readonly MongoDbContainer _container = new MongoDbBuilder()
        .WithImage("mongo:latest")
        .Build();

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    [Fact]
    public async Task InsertAsync_ShouldSaveTickToRealMongoDb()
    {
        var client = new MongoClient(_container.GetConnectionString());

        var options = Options.Create(new MongoDbSettings { DatabaseName = "TestDb" });

        var repository = new MongoMarketDataRepository(client, options);

        var tick = new MarketTick
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Ticker = "TSLA",
            Price = 200.5m,
            Timestamp = DateTime.UtcNow,
        };

        await repository.InsertASync(tick);

        var database = client.GetDatabase("TestDb");
        var collection = database.GetCollection<MarketTick>("MarketData");

        var savedTick = await collection.Find(t => t.Ticker == "TSLA").FirstOrDefaultAsync();

        savedTick.Should().NotBeNull();
        savedTick.Price.Should().Be(200.5m);
    }
}
