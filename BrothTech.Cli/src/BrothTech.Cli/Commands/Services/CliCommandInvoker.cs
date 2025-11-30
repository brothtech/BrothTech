using BrothTech.Cli.Commands.Root;
using BrothTech.Cli.Shared.Commands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BrothTech.Cli.Commands.Services;

public class CliCommandInvoker(
    IMemoryCache memoryCache,
    ILogger<CliCommandInvoker> logger) :
    ICliCommandInvoker
{
    private readonly IMemoryCache _memoryCache = memoryCache.EnsureNotNull();
    private readonly ILogger<CliCommandInvoker> _logger = logger.EnsureNotNull();

    public async Task<Result> TryInvokeAsync(
        string[] args,
        CancellationToken token = default)
    {
        if (_memoryCache.TryGetValue<RootCliCommand>(nameof(RootCliCommand), out var rootCommand) is false ||
            rootCommand is null)
            return ErrorResult.FromMessages(("Unable to get {command}", nameof(RootCliCommand)));

        var result = rootCommand.Parse(args);
        return await result.InvokeAsync(cancellationToken: token);
    }
}
