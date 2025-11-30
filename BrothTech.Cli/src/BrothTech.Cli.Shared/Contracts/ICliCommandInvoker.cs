using BrothTech.Contracts.Results;

namespace BrothTech.Cli.Shared.Contracts;

public interface ICliCommandInvoker
{
    public Task<Result> TryInvokeAsync(
        string[] args,
        CancellationToken token = default);
}
