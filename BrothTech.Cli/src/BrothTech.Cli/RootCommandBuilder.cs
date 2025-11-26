using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Commands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.Cli;

[ServiceDescriptor<RootCommandBuilder>]
public class RootCommandBuilder(
    ILogger<RootCommandBuilder> logger,
    IEnumerable<ICommandHandler<RootCliCommand, RootCliCommandResult>> handlers,
    IEnumerable<ICommandBuilder<RootCliCommand>> builders) :
    BaseCommandBuilder<RootCliCommand, RootCliCommand, RootCliCommandResult>(logger, handlers, builders)
{
    protected override bool IsRoot => true;
}
