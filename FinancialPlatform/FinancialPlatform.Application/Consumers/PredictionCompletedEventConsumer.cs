using System;
using System.Collections.Generic;
using System.Text;

using FinancialPlatform.Application.Interfaces;
using FinancialPlatform.Contracts;

using MassTransit;

namespace FinancialPlatform.Application.Consumers;

public class PredictionCompletedEventConsumer : IConsumer<PredictionCompletedEvent>
{
    private readonly IPredictionNotifier _predictionNotifier;

    public PredictionCompletedEventConsumer(IPredictionNotifier predictionNotifier)
    {
        _predictionNotifier = predictionNotifier;
    }

    public async Task Consume(ConsumeContext<PredictionCompletedEvent> context)
    {
        await _predictionNotifier.BroadcastPredictionAsync(context.Message);
        Console.WriteLine($"[RabbitMQ] Prediction for ticker {context.Message.Ticker} completed and sent to notification service.");
    }
}
