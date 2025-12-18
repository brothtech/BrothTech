using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands.Batch;

[BatchCommandMembers]
public class BatchCommand :
    Command
{
    public BatchCommand() : 
        base(nameof(BatchCommand))
    {
        Aliases.Add("batch");
    }
}
