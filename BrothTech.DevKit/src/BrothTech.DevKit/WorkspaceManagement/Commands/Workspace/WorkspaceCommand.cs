using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Workspace;

public class WorkspaceCommand :
    Command
{
    public WorkspaceCommand() :
        base(nameof(WorkspaceCommand))
    {
        Aliases.Add("workspace");
    }
}
