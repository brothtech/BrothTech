using System.Text.Json.Serialization;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Workspaces;

public class DomainInfo
{
    public string? ParentDomainName { get; set; }

    public required string Name { get; set; }

    public required string FullyQualifiedName { get; set; }

    public ProjectInfo[] Projects { get; set; } = [];

    [JsonIgnore]
    public WorkspaceInfo? Workspace { get; set; }
}
