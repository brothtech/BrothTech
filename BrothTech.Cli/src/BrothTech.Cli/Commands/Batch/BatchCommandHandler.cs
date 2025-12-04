using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using BrothTech.Infrastructure.DependencyInjection;
using System.Text.RegularExpressions;

namespace BrothTech.Cli.Commands.Batch;

[ServiceDescriptor<ICommandHandler<BatchCommand, BatchCommandResult>, BatchCommandHandler>]
public partial class BatchCommandHandler :
    ICommandHandler<BatchCommand, BatchCommandResult>
{
    [GeneratedRegex(@"""(?<value>[^""]*)""|(?<value>\S+)")]
    private static partial Regex GetSplitBatchRegex();

    public int Priority => 0;

    public Task<Result> TryHandleAsync(
        BatchCommandResult commandResult, 
        CancellationToken token)
    {
        return Task.FromResult(Result.Success);
    }

    public bool ShouldInvokeNewCommands(
        BatchCommandResult commandResult)
    {
        return commandResult.Batches.Length > 0;
    }

    public IEnumerable<string[]> GetNewCommandsArgs(
        BatchCommandResult commandResult)
    {
        var regex = GetSplitBatchRegex();
        foreach (var batch in commandResult.Batches)
            yield return [.. GetNewCommandArgs(regex, batch)];
    }

    private IEnumerable<string> GetNewCommandArgs(
        Regex regex,
        string batch)
    {
        foreach (Match match in regex.Matches(batch))
            yield return match.Groups["value"].Value;
    }
}