using BrothTech.Cli.Shared.Contracts.Commands;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Workspace;

public class WorkspaceCliCommand() :
    CliCommand(nameof(WorkspaceCliCommand))
{
    protected override IEnumerable<string> GetAliases()
    {
        yield return "workspace";
    }
}
