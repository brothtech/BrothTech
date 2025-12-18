using BrothTech.Cli.Shared.Contracts.Commands;
using System.CommandLine;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands;

public class DomainNameOptionAttribute :
    CliCommandMembersAttribute
{
    public Option<string> DomainName { get; } = new($"--{nameof(DomainName)}", "-d", "--domain");

    protected override Dictionary<string, Symbol> Members => field ??= new()
    {
        [nameof(DomainName)] = DomainName
    };
}
