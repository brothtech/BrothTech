using BrothTech.Cli.Shared.Contracts.Commands.Batch.Members;

namespace BrothTech.Cli.Shared.Contracts.Commands.Batch;

public class BatchRequest :
    ICliRequest<BatchCommand>
{
    [BatchSourcePathOption]
    public string? BatchSourcePath { get; set; }

    [BatchesOption]
    public string[] Batches { get; set; } = [];
}