using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;

namespace BrothTech.DevKit.WorkspaceManagement.Services;

public interface IWorkspaceManagementService
{
    Result<string> TryGetWorkspaceRootPath();

    Task<Result> TryAddDomainAsync(
        string domainName,
        DotNetProjectTemplate template,
        string? workspaceRootPath = null,
        CancellationToken token = default);
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

    public async Task<Result> TryAddDomainAsync(
        string domainName,
        DotNetProjectTemplate template,
        string? workspaceRootPath = null,
        CancellationToken token = default)
    {
        if (workspaceRootPath is null && TryGetWorkspaceRootPath().HasNoItem(out workspaceRootPath, out var messages))
            return ErrorResult.FromMessages([.. messages]);

        return await TryCreateDomainSolutionAsync(workspaceRootPath, domainName, token) &&
               await TryCreateDefaultDomainProjectsAsync(workspaceRootPath, domainName, template, token) &&
               await TryAddDomainProjectsToDomainSolutionAsync(workspaceRootPath, domainName, token);
    }

    private async Task<Result> TryCreateDomainSolutionAsync(
        string workspaceRootPath,
        string domainName,
        CancellationToken token)
    {
        var path = @$"{workspaceRootPath}\{domainName}";
        _fileSystemService.EnsureDirectoryExists(path);
        return await _dotNetService.TryCreateSolutionAsync(domainName, path, token);
    }

    private async Task<Result> TryCreateDefaultDomainProjectsAsync(
        string workspaceRootPath,
        string domainName,
        DotNetProjectTemplate template,
        CancellationToken token)
    {
        var srcPath = @$"{workspaceRootPath}\{domainName}\src\{domainName}";
        _fileSystemService.EnsureDirectoryExists(srcPath);
        return await _dotNetService.TryCreateProject(domainName, template, srcPath, token) &&
               await _dotNetService.TryCreateProject($"{domainName}.Shared", template, @$"{srcPath}.Shared", token) &&
               await _dotNetService.TryAddProjectReference(
                   projectPath: @$"{srcPath}.Shared\{domainName}.Shared.csproj",
                   referencePath: @$"{srcPath}\{domainName}.csproj",
                   token: token);
    }

    private async Task<Result> TryAddDomainProjectsToDomainSolutionAsync(
        string workspaceRootPath,
        string domainName,
        CancellationToken token)
    {
        var path = @$"{workspaceRootPath}\{domainName}";
        return await _dotNetService.TryAddProjectToSolution(
            solutionPath: @$"{path}\{domainName}.sln",
            projectPath: @$"{path}\src\{domainName}\{domainName}.csproj",
            token);
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

    public ProjectInfo[] Projects { get; set; } = [];
}

public class ProjectInfo
{
    public required string DomainName { get; set; }

    public required string Name { get; set; }

    public required ProjectExposureType ExposureType { get; set; }
}

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
                _ => false
            },
            ProjectExposureType.Public => target switch
            {
                ProjectExposureType.Internal => relationType is DomainRelationType.Descendent,
                ProjectExposureType.Public => relationType is DomainRelationType.Descendent,
                ProjectExposureType.Endpoint => true,
                ProjectExposureType.Sandbox => relationType is DomainRelationType.Descendent or
                                                               DomainRelationType.IntraDomainSibling,
                _ => false
            },
            ProjectExposureType.Shared => target switch
            {
                ProjectExposureType.Internal => relationType is not DomainRelationType.Ancestor,
                ProjectExposureType.Public => relationType is not DomainRelationType.Ancestor,
                ProjectExposureType.Shared => relationType is DomainRelationType.Descendent,
                ProjectExposureType.Endpoint => true,
                ProjectExposureType.Sandbox => relationType is DomainRelationType.Descendent or
                                                               DomainRelationType.IntraDomainSibling,
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
                ProjectExposureType.Shared => relationType is not DomainRelationType.Descendent,
                ProjectExposureType.Public => relationType is DomainRelationType.Ancestor,
                _ => false
            },
            ProjectExposureType.Public => target switch
            {
                ProjectExposureType.Internal => relationType is DomainRelationType.IntraDomainSibling,
                ProjectExposureType.Shared => relationType is not DomainRelationType.Descendent,
                _ => false
            },
            ProjectExposureType.Shared => target switch
            {
                ProjectExposureType.Shared => relationType is DomainRelationType.Ancestor,
                _ => false
            },
            ProjectExposureType.Endpoint => target switch
            {
                ProjectExposureType.Public => relationType is not DomainRelationType.Descendent,
                ProjectExposureType.Shared => relationType is not DomainRelationType.Descendent,
                _ => false
            },
            ProjectExposureType.Sandbox => target switch
            {
                ProjectExposureType.Internal => relationType is DomainRelationType.IntraDomainSibling or
                                                                DomainRelationType.Ancestor,
                ProjectExposureType.Public => relationType is DomainRelationType.IntraDomainSibling or
                                                              DomainRelationType.Ancestor,
                ProjectExposureType.Shared => relationType is DomainRelationType.IntraDomainSibling or
                                                              DomainRelationType.Ancestor,
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