using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

using Microsoft.Extensions.Logging;

namespace FinancialPlatform.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName} with request: {@Request}", requestName, request);

        var stopWatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var responce = await next();

            stopWatch.Stop();
            _logger.LogInformation("Handled {RequestName} in {ElapsedMilliseconds}ms", requestName, stopWatch.ElapsedMilliseconds);
        }
        catch (Exception)
        {
            stopWatch.Stop();
            _logger.LogError("Error handling {RequestName} after {ElapsedMilliseconds}ms", requestName, stopWatch.ElapsedMilliseconds);

            throw;
        }

        return await next();
    }
}
