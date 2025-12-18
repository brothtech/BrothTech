using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands.Batch;

public class BatchCommandMembersAttribute :
    CliCommandMembersAttribute
{
    public Option<string[]> Batches { get; } = new($"--{nameof(Batches)}", "-b", "--batches");

    public Option<string> BatchSourcePath { get; } = new($"--{nameof(BatchSourcePath)}", "-s", "--source-path");

    protected override Dictionary<string, Symbol> Members => field ??= new()
    {
        [nameof(Batches)] = Batches,
        [nameof(BatchSourcePath)] = BatchSourcePath
    };
}