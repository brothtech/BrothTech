using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands;

public class NameArgumentAttribute :
    CliCommandMembersAttribute
{
    public Argument<string> Name { get; } = new(nameof(Name));

    protected override Dictionary<string, Symbol> Members => field ??= new()
    {
        [nameof(Name)] = Name
    };
}