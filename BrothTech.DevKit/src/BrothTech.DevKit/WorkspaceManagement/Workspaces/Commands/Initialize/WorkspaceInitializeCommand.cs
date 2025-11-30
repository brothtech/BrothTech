using BrothTech.DevKit.WorkspaceManagement.Domains.Commands.Add;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.Commands.Initialize;

public class WorkspaceInitializeCommand :
    BaseDomainAddCommand
{
    public WorkspaceInitializeCommand() :
        base(nameof(WorkspaceInitializeCommand))
    {
        Aliases.Add("initialize");
        Aliases.Add("init");
    }
}
