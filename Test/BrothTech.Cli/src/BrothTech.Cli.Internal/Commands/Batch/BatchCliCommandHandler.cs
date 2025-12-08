using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Batch;
using BrothTech.Shared.Contracts.Results;
using BrothTech.Shared.Contracts.Services;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using System.Text.RegularExpressions;

namespace BrothTech.Cli.Internal.Commands.Batch;

[ServiceDescriptor<ICliCommandHandler<BatchCommand, BatchCommandResult>, BatchCliCommandHandler>]
public partial class BatchCliCommandHandler(
    IFileSystemService fileSystemService) :
    ICliCommandHandler<BatchCommand, BatchCommandResult>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();

    [GeneratedRegex(@"""(?<value>[^""]*)""|(?<value>\S+)")]
    private static partial Regex GetSplitBatchRegex();

    public int Priority => 0;

    public Task<Result> TryHandleAsync(
        BatchCommandResult commandResult, 
        CancellationToken token)
    {
        if (TryGetBatches(commandResult).HasItem(out var batches, out var messages) is false)
            return Task.FromResult<Result>(ErrorResult.FromMessages(messages));

        commandResult.Batches = batches;
        return Task.FromResult(Result.Success);
    }

    private Result<string[]> TryGetBatches(
        BatchCommandResult commandResult)
    {
        if (commandResult.BatchSourcePath.IsNullOrWhiteSpace())
            return commandResult.Batches;

        var result = _fileSystemService.TryReadFile(commandResult.BatchSourcePath);
        if (result.HasItem(out var fileContents, out var messages) is false)
            return ErrorResult.FromMessages(messages);

        var batches = new List<string>(commandResult.Batches);
        foreach (var line in fileContents.EnumerateLines())
            batches.Add(line.ToString());

        return batches.ToArray();
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