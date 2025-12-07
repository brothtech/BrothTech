using BrothTech.Shared.Contracts.Results;

namespace BrothTech.Cli.Shared.CliCommands;

public interface ICliCommandHandler<TCommand, TCommandResult> :
    ICliCommandHandler
    where TCommand : class, ICliCommand, new()
    where TCommandResult : ICliCommandResult<TCommand>
{
    Task<Result> TryHandleAsync(
        TCommandResult commandResult,
        CancellationToken token);

    bool ShouldInvokeNewCommands(
        TCommandResult commandResult);

    IEnumerable<string[]> GetNewCommandsArgs(
        TCommandResult commandResult);
}

public interface ICliCommandHandler
{
    int Priority { get; }
}
