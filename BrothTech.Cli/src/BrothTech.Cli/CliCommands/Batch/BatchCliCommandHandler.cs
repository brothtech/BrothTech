using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.CliCommands.Batch;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.Infrastructure.DependencyInjection;
using System.Text.RegularExpressions;

namespace BrothTech.Cli.CliCommands.Batch;

[ServiceDescriptor<ICliCommandHandler<BatchCliCommand, BatchCliCommandResult>, BatchCliCommandHandler>]
public partial class BatchCliCommandHandler(
    IFileSystemService fileSystemService) :
    ICliCommandHandler<BatchCliCommand, BatchCliCommandResult>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();

    [GeneratedRegex(@"""(?<value>[^""]*)""|(?<value>\S+)")]
    private static partial Regex GetSplitBatchRegex();

    public int Priority => 0;

    public Task<Result> TryHandleAsync(
        BatchCliCommandResult commandResult, 
        CancellationToken token)
    {
        if (TryGetBatches(commandResult).HasItem(out var batches, out var messages) is false)
            return Task.FromResult<Result>(ErrorResult.FromMessages(messages));

        commandResult.Batches = batches;
        return Task.FromResult(Result.Success);
    }

    private Result<string[]> TryGetBatches(
        BatchCliCommandResult commandResult)
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
        BatchCliCommandResult commandResult)
    {
        return commandResult.Batches.Length > 0;
    }

    public IEnumerable<string[]> GetNewCommandsArgs(
        BatchCliCommandResult commandResult)
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