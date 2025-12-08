namespace BrothTech.WorkspaceManagement.Shared.Contracts.Workspaces;

public class WorkspaceInfo
{
    public required string Name { get; set; }

    public DomainInfo[] Domains { get; set; } = [];
}
