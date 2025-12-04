using System.CommandLine;

namespace BrothTech.Cli.Commands.Batch;

public class BatchCommand :
    Command
{
    public Argument<string[]> Batches { get; } = new(nameof(Batches));

    public BatchCommand() :
        base(nameof(BatchCommand))
    {
        Aliases.Add("batch");
        Add(Batches);
    }
}
