namespace FinancialPlatform.Contracts;

public record MarketDataCollectedEvent(string Ticker, decimal Price, DateTime TimeStamp);
