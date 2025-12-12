using BrothTech.Shared.Contracts.Results;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands;

public interface ICliRequestValidator<TCommand, TCommandRequest> :
    ICliRequestValidator
    where TCommand : Command, new()
    where TCommandRequest : ICliRequest<TCommand>
{
    Task<Result> ValidateAsync(
        TCommandRequest request,
        CancellationToken token = default);
}

public interface ICliRequestValidator
{
    int Priority { get; }
}