using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using System.CommandLine;

namespace BrothTech.DevKit.DomainManagement.Commands.Domain.Add;

[ServiceDescriptor<ICommandHandler<DomainAddCommand>, DomainAddCommandHandler>]
public class DomainAddCommandHandler :
    ICommandHandler<DomainAddCommand>
{
    public bool IsHandler(
        DomainAddCommand command)
    {
        return command is DomainAddCommand;
    }

    public Task HandleAsync(
        DomainAddCommand command,
        ParseResult parseResult, 
        CancellationToken token)
    {
        var name = parseResult.GetRequiredValue(command.Name);
        return Task.CompletedTask;
    }
}
