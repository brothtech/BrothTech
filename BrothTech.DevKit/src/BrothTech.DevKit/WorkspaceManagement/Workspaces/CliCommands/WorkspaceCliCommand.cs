using BrothTech.Cli.Shared.CliCommands;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.CliCommands;

public class WorkspaceCliCommand() :
    CliCommand(nameof(WorkspaceCliCommand))
{
    protected override IEnumerable<string> GetAliases()
    {
        yield return "workspace";
    }
}
