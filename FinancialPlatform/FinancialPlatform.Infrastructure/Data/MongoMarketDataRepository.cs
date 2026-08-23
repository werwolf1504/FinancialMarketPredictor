using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Domain.Entities;
using FinancialPlatform.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FinancialPlatform.Infrastructure.Data;

public class MongoMarketDataRepository
{
    private readonly IMongoCollection<MarketTick> _marketDataCollection;
    public MongoMarketDataRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> mongoDbSettings)
    {
        var database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _marketDataCollection = database.GetCollection<MarketTick>("MarketData");

        var indexKeysDefinition = Builders<MarketTick>.IndexKeys
                                                        .Ascending(tick => tick.Ticker)
                                                        .Descending(tick=>tick.Timestamp);

        var indexModel = new CreateIndexModel<MarketTick>(indexKeysDefinition);
        _marketDataCollection.Indexes.CreateOne(indexModel);
    }
}
