using System.Text.Json.Serialization;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Workspaces;

public class ProjectInfo
{
    public required string DomainName { get; set; }

    public required string Name { get; set; }

    public required string AssemblyName { get; set; }

    public required ProjectExposureType ExposureType { get; set; }

    public PackageInfo[] PackageReferences { get; set; } = [];

    [JsonIgnore]
    public DomainInfo? Domain { get; set; }
}
