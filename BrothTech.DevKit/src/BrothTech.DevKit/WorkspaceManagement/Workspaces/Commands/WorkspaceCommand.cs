using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.Commands;

public class WorkspaceCommand :
    Command
{
    public WorkspaceCommand() :
        base(nameof(WorkspaceCommand))
    {
        Aliases.Add("workspace");
    }
}
