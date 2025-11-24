using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts;

public interface ICommandHandler<TCommand> :
    ICommandHandler
{
    Task HandleAsync(
        TCommand command,
        ParseResult parseResult,
        CancellationToken token);
}

public interface ICommandHandler;