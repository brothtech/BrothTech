using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.WorkspaceManagement.Shared.Contracts.Workspaces;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Project.Add;

public class ProjectAddRequest :
    ICliRequest<ProjectAddCommand>
{
    public string Name { get; set; } = default!;

    public ProjectExposureType ExposureType { get; set; }

    public string? Template { get; set; }

    public string? FullyQualifiedName { get; set; }

    public string? DomainName { get; set; }

    public string? WorkspacePath { get; set; }
}