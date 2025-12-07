using System.CommandLine;

namespace BrothTech.Cli.Shared.CliCommands.Batch;

public class BatchCliCommand :
    CliCommand
{
    public Option<string> BatchSourcePath => GetOrCreateOption<string>(nameof(BatchSourcePath), "-s", "--source-path");

    public Option<string[]> Batches => GetOrCreateOption<string[]>(nameof(Batches), "-b", "--batches");

    public BatchCliCommand() : 
        base(nameof(BatchCliCommand))
    {
        AddOption(BatchSourcePath);
        AddOption(Batches);
    }

    protected override IEnumerable<string> GetAliases()
    {
        yield return "batch";
    }
}
