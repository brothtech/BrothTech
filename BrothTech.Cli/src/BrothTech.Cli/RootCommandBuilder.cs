using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace BrothTech.Cli;

[ServiceDescriptor<ICommandBuilder, RootCommandBuilder>]
public class RootCommandBuilder(
    ILogger<RootCommandBuilder> logger,
    IEnumerable<ICommandHandler<RootCommand>> handlers,
    IEnumerable<ICommandBuilder<RootCommand>> builders) :
    BaseCommandBuilder<RootCommand>(logger, handlers, builders)
{
    protected override bool IsRoot => true;

    public override bool IsChild(
        Command command)
    {
        return false;
    }

    protected override RootCommand BuildInternal()
    {
        return new RootCommand();
    }
}