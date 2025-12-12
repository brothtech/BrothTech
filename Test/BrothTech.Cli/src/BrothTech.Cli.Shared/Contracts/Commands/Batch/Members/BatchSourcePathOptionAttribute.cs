using BrothTech.Cli.Shared.Contracts.Commands.Members;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands.Batch.Members;

public class BatchSourcePathOptionAttribute :
    CliOptionAttribute<string>
{
    private static readonly Lazy<Option<string>> _option = new(() => new("BatchSourcePath", "-s", "--source-path"));

    public static Option<string> BatchSourcePath => _option.Value;

    public override Option Option => BatchSourcePath;
}
