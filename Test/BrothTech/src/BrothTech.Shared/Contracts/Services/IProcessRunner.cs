using BrothTech.Shared.Contracts.Results;

namespace BrothTech.Shared.Contracts.Services;

public interface IProcessRunner
{
    Task<Result> TryRunAsync(
        string fileName,
        CancellationToken token,
        params string[] args);
}
