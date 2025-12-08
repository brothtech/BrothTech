using BrothTech.Contracts.Results;

namespace BrothTech.Cli.Shared.CliCommands;

public interface ICliCommandValidator<TCommand, TCommandResult> :
    ICliCommandValidator
    where TCommand : class, ICliCommand, new()
    where TCommandResult : ICliCommandResult<TCommand>
{
    Task<Result> ValidateAsync(
        TCommandResult command,
        CancellationToken token = default);
}

public interface ICliCommandValidator
{
}