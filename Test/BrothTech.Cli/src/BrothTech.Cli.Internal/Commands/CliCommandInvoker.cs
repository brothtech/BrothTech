using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Root;
using BrothTech.Shared.Contracts.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BrothTech.Cli.Internal.Commands;

public partial class CliCommandInvoker(
    ILogger<CliCommandInvoker> logger,
    IMemoryCache memoryCache) :
    ICliCommandInvoker
{
    private readonly IMemoryCache _memoryCache = memoryCache.EnsureNotNull();
    private readonly ILogger<CliCommandInvoker> _logger = logger.EnsureNotNull();

    [LoggerMessage("Started invoking command at {utcNow}.", Level = LogLevel.Information)]
    public static partial void LogCommandInvocationStart(ILogger logger, DateTime utcNow);

    [LoggerMessage("Ended invoking command after {elapsed}.", Level = LogLevel.Information)]
    public static partial void LogCommandInvocationEnd(ILogger logger, TimeSpan elapsed);

    [LoggerMessage("An error occurred invoking command after {elapsed}.", Level = LogLevel.Error)]
    public static partial void LogCommandInvocationError(ILogger logger, Exception exception, TimeSpan elapsed);

    public async Task<Result> TryInvokeAsync(
        string[] args,
        CancellationToken token = default)
    {
        LogCommandInvocationStart(_logger, DateTime.UtcNow);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var rootCommand = GetRootCommand();
            var parseResult = rootCommand.Parse(args);
            var result = await parseResult.InvokeAsync(cancellationToken: token);
            LogCommandInvocationEnd(_logger, stopwatch.Elapsed);
            return result;
        }
        catch (Exception exception)
        {
            LogCommandInvocationError(_logger, exception, stopwatch.Elapsed);
            return Result.Failure;
        }
    }

    private RootCliCommand GetRootCommand()
    {
        if (_memoryCache.TryGetValue<RootCliCommand>(nameof(RootCliCommand), out var command))
            return command.EnsureNotNull();

        throw new InvalidOperationException($"Unable to retrieve {nameof(RootCliCommand)}");
    }
}
