using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.CliCommands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BrothTech.Cli.CliCommands.Root;

[ServiceDescriptor<RootCliCommandBuilder>]
public class RootCliCommandBuilder(
    ILogger<RootCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<RootCliCommand>> builders,
    IEnumerable<ICliCommandValidator<RootCliCommand, RootCliCommandResult>> validators,
    IEnumerable<ICliCommandHandler<RootCliCommand, RootCliCommandResult>> handlers,
    ICliCommandInvoker commandInvoker,
    IMemoryCache memoryCache) :
    CliCommandBuilder<RootCliCommand, RootCliCommand, RootCliCommandResult, RootCliCommandResult>(
        logger, 
        builders, 
        validators,
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