namespace FinancialPlatform.Contracts;

public record MarketDataCollectedEvent(string Ticker, int Price, DateTime TimeStamp);
