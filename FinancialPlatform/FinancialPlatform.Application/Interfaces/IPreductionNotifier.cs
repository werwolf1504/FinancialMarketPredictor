using FinancialPlatform.Contracts;

namespace FinancialPlatform.Application.Interfaces;

public interface IPredictionNotifier
{
    Task BroadcastPredictionAsync(PredictionCompletedEvent prediction);
}