using BrothTech.Cli.Shared.CliCommands;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands.Add;

public class DomainAddCliCommand :
    CliCommand,
    IBaseDomainAddCliCommand
{
    public Option<string> ParentDomainName { get; } = new($"--{nameof(ParentDomainName)}", "-p", "--parent");

    public DomainAddCliCommand() : 
        base(nameof(DomainAddCliCommand))
    {
        ((IBaseDomainAddCliCommand)this).Add();
        AddOption(ParentDomainName);
    }

    protected override IEnumerable<string> GetAliases()
    {
        yield return "add";
    }
}
