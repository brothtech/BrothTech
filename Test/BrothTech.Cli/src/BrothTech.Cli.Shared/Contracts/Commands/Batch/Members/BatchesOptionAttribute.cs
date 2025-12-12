using BrothTech.Cli.Shared.Contracts.Commands.Members;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands.Batch.Members;

public class BatchesOptionAttribute :
    CliOptionAttribute<string[]>
{
    private static readonly Lazy<Option<string[]>> _option = new(() => new("Batches", "-b", "--batches"));

    public static Option<string[]> Batches => _option.Value;

    public override Option Option => Batches;
}