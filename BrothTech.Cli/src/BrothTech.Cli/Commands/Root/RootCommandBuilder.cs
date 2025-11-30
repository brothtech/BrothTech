using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Commands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.Cli.Commands.Root;

[ServiceDescriptor<RootCommandBuilder>]
public class RootCommandBuilder(
    ILogger<RootCommandBuilder> logger,
    IEnumerable<ICommandBuilder<RootCliCommand>> builders,
    IEnumerable<ICommandHandler<RootCliCommand, RootCliCommandResult>> handlers) :
    BaseCommandBuilder<RootCliCommand, RootCliCommand, RootCliCommandResult>(
        logger, 
        builders, 
        handlers, 
        new ExplicitCliCommandInvoker());

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