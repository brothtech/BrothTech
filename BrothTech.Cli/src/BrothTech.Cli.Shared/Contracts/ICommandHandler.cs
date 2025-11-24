using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts;

public interface ICommandHandler<TCommand> :
    ICommandHandler
{
    bool IsHandler(
        TCommand command);

    Task HandleAsync(
        TCommand command,
        ParseResult parseResult,
        CancellationToken token);
}

public interface ICommandHandler;