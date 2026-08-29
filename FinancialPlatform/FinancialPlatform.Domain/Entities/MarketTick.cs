using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace FinancialPlatform.Domain.Entities;

public class MarketTick
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Ticker { get; set; }
    public decimal Price { get; set; }
    public long Volume { get; set; }
    public DateTime Timestamp { get; set; }
}
