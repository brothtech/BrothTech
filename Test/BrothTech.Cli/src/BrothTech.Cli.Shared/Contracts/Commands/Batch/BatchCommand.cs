using BrothTech.Cli.Shared.Contracts.Commands.Batch.Members;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands.Batch;

[BatchSourcePathOption, BatchesOption]
public class BatchCommand :
    Command
{
    public BatchCommand() : 
        base(nameof(BatchCommand))
    {
        Aliases.Add("batch");
    }
}