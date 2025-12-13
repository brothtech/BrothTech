using BrothTech.Cli.Shared.Contracts.Commands.Root;
using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using BrothTech.Cli.Shared;

namespace BrothTech.Cli.Internal.Commands.Root;

[ServiceDescriptor<RootCliCommandBuilder>]
internal class RootCliCommandBuilder(
    ILogger<RootCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<RootCliCommand>> childBuilders,
    ICliRequestInvoker requestInvoker,
    IMemoryCache memoryCache) :
    CliCommandBuilder<RootCliCommand, RootCliCommand, RootCliRequest>(
        logger, 
        childBuilders, 
        requestInvoker)
{
    private readonly IMemoryCache _memoryCache = memoryCache.EnsureNotNull();

    protected override void OnBuilt(
        RootCliCommand command)
    {
        _memoryCache.Set(nameof(RootCliCommand), command);
    }
}