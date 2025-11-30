using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Commands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BrothTech.Cli.Commands.Root;

[ServiceDescriptor<RootCommandBuilder>]
public class RootCommandBuilder(
    ILogger<RootCommandBuilder> logger,
    IEnumerable<ICommandBuilder<RootCliCommand>> builders,
    IEnumerable<ICommandHandler<RootCliCommand, RootCliCommandResult>> handlers,
    ICliCommandInvoker commandInvoker,
    IMemoryCache memoryCache) :
    BaseCommandBuilder<RootCliCommand, RootCliCommand, RootCliCommandResult>(
        logger, 
        builders, 
        handlers,
        commandInvoker)
{
    private readonly IMemoryCache _memoryCache = memoryCache.EnsureNotNull();

    protected override void OnBuilt(
        RootCliCommand command)
    {
        _memoryCache.Set(nameof(RootCliCommand), command);
    }
}

file class ExplicitCliCommandInvoker :
    ICliCommandInvoker
{
    public Task<Result> TryInvokeAsync(
        string[] args, 
        CancellationToken token = default)
    {
        return Task.FromResult(Result.Success);
    }
}