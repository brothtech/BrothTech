using BrothTech.Shared.Contracts.Results;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands;

public interface ICliRequestInvoker
{
    public Task<Result> TryInvokeAsync<TCommand, TRequest>(
        TRequest request,
        CancellationToken token = default)
        where TCommand: Command, new()
        where TRequest : ICliRequest<TCommand>;
}
