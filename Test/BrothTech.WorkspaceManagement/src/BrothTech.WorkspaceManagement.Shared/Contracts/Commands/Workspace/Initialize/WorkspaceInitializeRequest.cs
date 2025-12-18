using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.WorkspaceManagement.Shared.Contracts.Workspaces;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Workspace.Initialize;

public class WorkspaceInitializeRequest :
    ICliRequest<WorkspaceInitializeCommand>
{
    public string Name { get; set; } = default!;

    public ProjectExposureType ExposureType { get; set; }

    public string? Template { get; set; }

    public string? FullyQualifiedName { get; set; }

    public string? DomainName { get; set; }

    public string? WorkspacePath { get; set; }

    public bool ShouldAddSharedProject { get; set; }

    public bool ShouldAddSandboxProject { get; set; }

    public bool ShouldAddInternalProject { get; set; }
}