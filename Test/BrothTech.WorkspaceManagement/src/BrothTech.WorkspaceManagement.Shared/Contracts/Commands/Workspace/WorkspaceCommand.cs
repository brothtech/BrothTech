using System.CommandLine;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Workspace;

public class WorkspaceCommand :
    Command
{
    public WorkspaceCommand() : 
        base(nameof(WorkspaceCommand))
    {
        Aliases.Add("workspace");
    }
}
