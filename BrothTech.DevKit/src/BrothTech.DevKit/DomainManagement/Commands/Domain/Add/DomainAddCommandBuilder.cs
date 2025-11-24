using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace BrothTech.DevKit.DomainManagement.Commands.Domain.Add;

[ServiceDescriptor<ICommandBuilder<DomainCommand>, DomainAddCommandBuilder>]
public class DomainAddCommandBuilder(
    ILogger<DomainAddCommandBuilder> logger,
    IEnumerable<ICommandHandler<DomainAddCommand>> handlers,
    IEnumerable<ICommandBuilder<DomainAddCommand>> builders) :
    BaseCommandBuilder<DomainAddCommand>(logger, handlers, builders),
    ICommandBuilder<DomainCommand>
{
    public override bool IsChild(
        Command command)
    {
        return command is DomainCommand;
    }

    protected override DomainAddCommand BuildInternal()
    {
        return new DomainAddCommand();
    }
}