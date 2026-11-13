using System.Text.Json.Serialization;

namespace FinancialPlatform.Contracts;

public class PredictionCompletedEvent
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal PredictedPrice { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}