using BrothTech.Contracts.Results;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts;

public interface ICommandHandler<TCommand, TCommandResult> :
    ICommandHandler
    where TCommand : Command
    where TCommandResult : ICommandResult<TCommand>
{
    Task<Result> TryHandleAsync(
        TCommandResult commandResult,
        CancellationToken token);

    bool ShouldInvokeNewCommand(
        TCommandResult commandResult);

    IEnumerable<string> GetNewCommandArgs(
        TCommandResult commandResult);
}

public interface ICommandHandler
{
    int Priority { get; }
}
