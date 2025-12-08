using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Root;
using BrothTech.Shared.Contracts.Results;
using Microsoft.Extensions.Caching.Memory;

namespace BrothTech.Cli.Internal.Commands;

public class CliCommandInvoker(
    IMemoryCache memoryCache) :
    ICliCommandInvoker
{
    private readonly IMemoryCache _memoryCache = memoryCache.EnsureNotNull();

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
