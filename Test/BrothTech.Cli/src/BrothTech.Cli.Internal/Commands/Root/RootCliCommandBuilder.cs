using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Root;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BrothTech.Cli.Internal.Commands.Root;

[ServiceDescriptor<RootCliCommandBuilder>]
public class RootCliCommandBuilder(
    ILogger<RootCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<RootCliCommand>> builders,
    IEnumerable<ICliCommandHandler<RootCliCommand, RootCliCommandResult>> handlers,
    ICliCommandInvoker commandInvoker,
    IMemoryCache memoryCache) :
    CliCommandBuilder<RootCliCommand, RootCliCommand, RootCliCommandResult, RootCliCommandResult>(
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