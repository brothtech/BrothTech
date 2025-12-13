using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Shared;
using BrothTech.Shared.Contracts.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Diagnostics;

namespace BrothTech.Cli.Internal.Commands;

internal partial class CliRequestInvoker(
    ILogger<CliRequestInvoker> logger,
    IServiceScopeFactory scopeFactory) :
    ICliRequestInvoker
{
    private readonly ILogger<CliRequestInvoker> _logger = logger.EnsureNotNull();
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory.EnsureNotNull();

    [LoggerMessage("Started invoking request {requestName} at {utcNow}.", Level = LogLevel.Information)]
    public static partial void LogRequestInvocationStart(ILogger logger, string requestName, DateTime utcNow);

    [LoggerMessage("Ended invoking request {requestName} after {elapsed}.", Level = LogLevel.Information)]
    public static partial void LogRequestInvocationEnd(ILogger logger, string requestName, TimeSpan elapsed);

    [LoggerMessage("An error occurred invoking request {requestName} after {elapsed}.", Level = LogLevel.Error)]
    public static partial void LogRequestInvocationError(ILogger logger, Exception exception, string requestName, TimeSpan elapsed);

    public async Task<Result> TryInvokeAsync<TCommand, TRequest>(
        TRequest request, 
        CancellationToken token = default)
        where TCommand : Command, new()
        where TRequest : ICliRequest<TCommand>
    {
        LogRequestInvocationStart(_logger, typeof(TRequest).Name, DateTime.UtcNow);
        var stopwatch = Stopwatch.StartNew();
        using var scope = _scopeFactory.CreateScope();
        try
        {
            var result = await TryInvokeAsync<TCommand, TRequest>(request, scope, token);
            LogRequestInvocationEnd(_logger, typeof(TRequest).Name, stopwatch.Elapsed);
            return result;
        } 
        catch (Exception exception)
        {
            LogRequestInvocationError(_logger, exception, typeof(TRequest).Name, stopwatch.Elapsed);
            return Result.Failure;
        }
    }

    private async Task<Result> TryInvokeAsync<TCommand, TRequest>(
        TRequest request,
        IServiceScope scope,
        CancellationToken token)
        where TCommand : Command, new()
        where TRequest : ICliRequest<TCommand>
    {
        var validators = GetValidators<TCommand, TRequest>(scope);
        var handlers = GetHandlers<TCommand, TRequest>(scope);
        return await validators.AggregateResultsAsync(async x => await x.ValidateAsync(request, token)) &&
               await handlers.AggregateResultsAsync(async x => await x.TryHandleAsync(request, token));
    }

    private IEnumerable<ICliRequestValidator<TCommand, TRequest>> GetValidators<TCommand, TRequest>(
        IServiceScope scope)
        where TCommand : Command, new()
        where TRequest : ICliRequest<TCommand>
    {
        var services = scope.ServiceProvider;
        var validators = services.GetServices<ICliRequestValidator<TCommand, TRequest>>();
        return validators.OrderBy(x => x.Priority);
    }

    private IEnumerable<ICliRequestHandler<TCommand, TRequest>> GetHandlers<TCommand, TRequest>(
        IServiceScope scope)
        where TCommand : Command, new()
        where TRequest : ICliRequest<TCommand>
    {
        var services = scope.ServiceProvider;
        var handlers = services.GetServices<ICliRequestHandler<TCommand, TRequest>>();
        return handlers.OrderBy(x => x.Priority);
    }
}