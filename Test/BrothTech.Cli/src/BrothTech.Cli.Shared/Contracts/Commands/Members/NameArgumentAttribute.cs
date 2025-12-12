using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands.Members;

public class NameArgumentAttribute :
    CliArgumentAttribute<string>
{
    private static readonly Lazy<Argument<string>> _argument = new(() => new(nameof(Name)));

    public static Argument<string> Name => _argument.Value;

    public override Argument Argument => Name;
}
