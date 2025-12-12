using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Batch;
using BrothTech.Shared;
using BrothTech.Shared.Contracts.Results;
using BrothTech.Shared.Contracts.Services;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using System.Text.RegularExpressions;

namespace BrothTech.Cli.Internal.Commands.Batch;

[ServiceDescriptor<ICliRequestHandler<BatchCommand, BatchRequest>, BatchRequestHandler>]
public partial class BatchRequestHandler(
    IFileSystemService fileSystemService,
    ICliCommandInvoker commandInvoker) :
    ICliRequestHandler<BatchCommand, BatchRequest>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly ICliCommandInvoker _commandInvoker = commandInvoker.EnsureNotNull();

    [GeneratedRegex(@"""(?<value>[^""]*)""|(?<value>\S+)")]
    private static partial Regex GetSplitBatchRegex();

    public int Priority => 0;

    public async Task<Result> TryHandleAsync(
        BatchRequest request, 
        CancellationToken token)
    {
        if (TryGetBatches(request).HasItem(out var batches, out var messages) is false)
            return ErrorResult.FromMessages(messages);

        var commandsArgs = GetNewCommandsArgs(batches);
        return await commandsArgs.AggregateResultsAsync(async x => await _commandInvoker.TryInvokeAsync(x, token));
    }

    private Result<string[]> TryGetBatches(
        BatchRequest request)
    {
        if (request.BatchSourcePath.IsNullOrWhiteSpace())
            return request.Batches;

        var result = _fileSystemService.TryReadFile(request.BatchSourcePath);
        if (result.HasItem(out var fileContents, out var messages) is false)
            return ErrorResult.FromMessages(messages);

        var batches = new List<string>(request.Batches);
        foreach (var line in fileContents.EnumerateLines())
            batches.Add(line.ToString());

        return batches.ToArray();
    }

    public IEnumerable<string[]> GetNewCommandsArgs(
        string[] batches)
    {
        var regex = GetSplitBatchRegex();
        foreach (var batch in batches)
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