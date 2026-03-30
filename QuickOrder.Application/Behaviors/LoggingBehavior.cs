using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace QuickOrder.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Ejecutando {Request}: {@RequestData}", requestName, request);

        var sw = Stopwatch.StartNew();
        try
        {
            var response = await next();
            sw.Stop();

            logger.LogInformation("Completado {Request} en {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "Error en {Request} después de {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
