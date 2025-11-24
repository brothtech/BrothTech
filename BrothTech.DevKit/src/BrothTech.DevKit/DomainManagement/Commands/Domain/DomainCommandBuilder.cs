using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace BrothTech.DevKit.DomainManagement.Commands.Domain;

[ServiceDescriptor<ICommandBuilder<RootCommand>, DomainCommandBuilder>]
public class DomainCommandBuilder(
    ILogger<DomainCommandBuilder> logger,
    IEnumerable<ICommandHandler<DomainCommand>> handlers,
    IEnumerable<ICommandBuilder<DomainCommand>> builders) :
    BaseCommandBuilder<DomainCommand>(logger, handlers, builders),
    ICommandBuilder<RootCommand>
{
    public override bool IsChild(
        Command command)
    {
        return command is RootCommand;
    }

    protected override DomainCommand BuildInternal()
    {
        return new DomainCommand();
    }
}
