using BrothTech.Shared.Contracts.Results;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands;

public interface ICliRequestHandler<TCommand, TRequest> :
    ICliRequestHandler
    where TCommand : Command, new()
    where TRequest : ICliRequest<TCommand>
{
    Task<Result> TryHandleAsync(
        TRequest request,
        CancellationToken token);
}

public interface ICliRequestHandler
{
    int Priority { get; }
}
