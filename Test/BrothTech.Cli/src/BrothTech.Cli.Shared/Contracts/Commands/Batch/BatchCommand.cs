using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands.Batch;

public class BatchCommand :
    CliCommand
{
    public Option<string> BatchSourcePath => GetOrCreateOption<string>(nameof(BatchSourcePath), "-s", "--source-path");

    public Option<string[]> Batches => GetOrCreateOption<string[]>(nameof(Batches), "-b", "--batches");

    public BatchCommand() :
        base(nameof(BatchCommand))
    {
        AddOption(BatchSourcePath);
        AddOption(Batches);
    }

    protected override IEnumerable<string> GetAliases()
    {
        yield return "batch";
    }
}
