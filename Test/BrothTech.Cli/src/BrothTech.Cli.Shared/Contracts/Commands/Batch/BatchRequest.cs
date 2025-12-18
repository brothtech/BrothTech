namespace BrothTech.Cli.Shared.Contracts.Commands.Batch;

public class BatchRequest :
    ICliRequest<BatchCommand>
{
    public string? BatchSourcePath { get; set; }

    public string[] Batches { get; set; } = [];
}