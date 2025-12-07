using BrothTech.Cli.Shared.CliCommands;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands;

public class DomainCliCommand() :
    CliCommand(nameof(DomainCliCommand))
{
    protected override IEnumerable<string> GetAliases()
    {
        yield return "domain";
    }
}
