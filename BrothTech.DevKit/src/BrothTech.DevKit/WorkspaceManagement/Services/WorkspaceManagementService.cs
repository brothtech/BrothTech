using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;
using BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;
using System.Text.Json.Serialization;

namespace BrothTech.DevKit.WorkspaceManagement.Services;

public interface IWorkspaceManagementService
{
    Result<string> TryGetWorkspaceRootPath();
}

public class WorkspaceManagementService(
    IFileSystemService fileSystemService,
    IDotNetService dotNetService) :
    IWorkspaceManagementService
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IDotNetService _dotNetService = dotNetService.EnsureNotNull();

    public Result<string> TryGetWorkspaceRootPath()
    {
        if (_fileSystemService.TryFindFile(".workspace").HasFailed(out var file, out var messages))
            return ErrorResult.FromMessages([.. messages]);

        if (file?.Directory?.FullName is null)
            return ErrorResult.FromErrorMessages("Unexpected error finding root directory.");

        return file.Directory.FullName;
    }
}

public class WorkspaceInfo
{
    public required string Name { get; set; }

    public DomainInfo[] Domains { get; set; } = [];
}

public class DomainInfo
{
    public string? ParentDomainName { get; set; }
    
    public required string Name { get; set; }

    public required string FullyQualifiedName { get; set; }

    public ProjectInfo[] Projects { get; set; } = [];

    public string[] DomainReferences { get; set; } = [];

    [JsonIgnore]
    public WorkspaceInfo? Workspace { get; set; }
}

public class ProjectInfo
{
    public required string DomainName { get; set; }

    public required string Name { get; set; }

    public required ProjectExposureType ExposureType { get; set; }

    public PackageInfo[] PackageReferences { get; set; } = [];

    [JsonIgnore]
    public DomainInfo? Domain { get; set; }
}

public record PackageInfo(string Name, string Version);

public enum ProjectExposureType
{
    None,
    Internal,
    Public,
    Shared,
    Endpoint,
    Sandbox
}

public static class ProjectExposureTypeExtentions
{
    public static bool IsVisibleTo(
        this ProjectExposureType value,
        ProjectExposureType target,
        DomainRelationType relationType)
    {
        return value switch
        {
            ProjectExposureType.Internal => target switch
            {
                ProjectExposureType.Internal => relationType is DomainRelationType.IntraDomainSibling,
                ProjectExposureType.Public => relationType is DomainRelationType.IntraDomainSibling,
                ProjectExposureType.Sandbox => relationType is DomainRelationType.IntraDomainSibling,
                _ => false
            },
            ProjectExposureType.Public => target switch
            {
                ProjectExposureType.Internal => relationType is DomainRelationType.Ancestor or
                                                                DomainRelationType.InterDomainReferencee,
                ProjectExposureType.Public => relationType is DomainRelationType.Ancestor or
                                                              DomainRelationType.InterDomainReferencee,
                ProjectExposureType.Endpoint => true,
                ProjectExposureType.Sandbox => relationType is DomainRelationType.Ancestor or
                                                               DomainRelationType.IntraDomainSibling or
                                                               DomainRelationType.InterDomainReferencee,
                _ => false
            },
            ProjectExposureType.Shared => target switch
            {
                ProjectExposureType.Internal => relationType is not (DomainRelationType.Descendent or 
                                                                    DomainRelationType.InterDomainReferencer),
                ProjectExposureType.Public => relationType is not (DomainRelationType.Descendent or 
                                                                  DomainRelationType.InterDomainReferencer),
                ProjectExposureType.Shared => relationType is DomainRelationType.Ancestor or
                                                              DomainRelationType.InterDomainReferencee,
                ProjectExposureType.Endpoint => true,
                ProjectExposureType.Sandbox => relationType is DomainRelationType.Ancestor or
                                                               DomainRelationType.IntraDomainSibling or
                                                               DomainRelationType.InterDomainReferencee,
                _ => false
            },
            _ => false
        };
    }

    public static bool CanDependOn(
        this ProjectExposureType value,
        ProjectExposureType target,
        DomainRelationType relationType)
    {
        return value switch
        {
            ProjectExposureType.Internal => target switch
            {
                ProjectExposureType.Shared => relationType is not DomainRelationType.Descendent or 
                                                                  DomainRelationType.InterDomainReferencee,
                ProjectExposureType.Public => relationType is DomainRelationType.Descendent or
                                                              DomainRelationType.InterDomainReferencer,
                _ => false
            },
            ProjectExposureType.Public => target switch
            {
                ProjectExposureType.Internal => relationType is DomainRelationType.IntraDomainSibling,
                ProjectExposureType.Public => relationType is DomainRelationType.Descendent or 
                                                              DomainRelationType.InterDomainReferencer,
                ProjectExposureType.Shared => relationType is not DomainRelationType.Ancestor or
                                                                  DomainRelationType.InterDomainReferencee,
                _ => false
            },
            ProjectExposureType.Shared => target switch
            {
                ProjectExposureType.Shared => relationType is DomainRelationType.Descendent or 
                                                              DomainRelationType.InterDomainReferencer,
                _ => false
            },
            ProjectExposureType.Endpoint => target switch
            {
                ProjectExposureType.Internal => relationType is DomainRelationType.IntraDomainSibling,
                ProjectExposureType.Public => relationType is not (DomainRelationType.Ancestor or 
                                                                  DomainRelationType.InterDomainReferencee),
                ProjectExposureType.Shared => relationType is not (DomainRelationType.Ancestor or 
                                                                  DomainRelationType.InterDomainReferencee),
                _ => false
            },
            ProjectExposureType.Sandbox => target switch
            {
                ProjectExposureType.Internal => relationType is DomainRelationType.IntraDomainSibling,
                ProjectExposureType.Public => relationType is DomainRelationType.IntraDomainSibling or
                                                              DomainRelationType.Descendent or 
                                                              DomainRelationType.InterDomainReferencer,
                ProjectExposureType.Shared => relationType is DomainRelationType.IntraDomainSibling or
                                                              DomainRelationType.Descendent or 
                                                              DomainRelationType.InterDomainReferencer,
                _ => false
            },
            _ => false
        };
    }
}


/*
-project access modififers
    -internal
        -visible to
            -intra-domain internal projects
            -intra-domain public projects
        -can depend on
            -intra-domain shared projects
            -inter-domain shared projects
            -ancestor public projects
            -ancestor shared projects        
    -public
        -visible to
            -descendent internal projects
            -descendent public projects
            -{the access modifier name I want your help with}
        -can depend on
            -intra-domain internal projects
            -intra-domain shared projects
            -inter-domain shared projects
            -ancestor public projects
            -ancestor shared projects
    -shared
        -visible to
            -intra-domain internal projects
            -intra-domain public projects
            -inter-domain internal projects
            -inter-domain public projects
            -descendent internal projects            
            -descendent public projects
            -descendent shared projects
            -{the access modifier name I want your help with}
        -can depend on
            -ancestor shared projects
    -Endpoint
        -visible to
            -none
        -can depend on
            -intra-domain public projects
            -intra-domain shared projects
            -inter-domain public projects
            -inter-domain shared projects
            -ancestor public projects
            -ancestor shared projects
    -Sandbox
        -visible to
            -none
        -can depend on
            -intra-domain internal projects
            -intra-domain public projects
            -intra-domain shared projects
 */