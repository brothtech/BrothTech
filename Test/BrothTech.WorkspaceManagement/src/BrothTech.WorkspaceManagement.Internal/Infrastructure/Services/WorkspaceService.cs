using BrothTech.Shared.Contracts.Results;
using BrothTech.Shared.Contracts.Services;
using BrothTech.WorkspaceManagement.Shared.Contracts.Services;
using BrothTech.WorkspaceManagement.Shared.Contracts.Workspaces;
using Microsoft.Extensions.Caching.Memory;

namespace BrothTech.WorkspaceManagement.Internal.Infrastructure.Services;

public class WorkspaceService(
    IMemoryCache memoryCache,
    IFileSystemService fileSystemService) :
    IWorkspaceService
{
    private readonly IMemoryCache _memoryCache = memoryCache.EnsureNotNull();
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    
    public Result<string> TryGetWorkspaceDirectory()
    {
        if (_fileSystemService.TryFindFile(".workspace").HasFailed(out var file, out var messages))
            return ErrorResult.FromMessages([.. messages]);

        if (file?.Directory?.FullName is null)
            return ErrorResult.FromErrorMessages("Unexpected error finding root directory.");

        return file.Directory.FullName;
    }

    public string GetDomainDirectory(
        string workspacePath,
        DomainInfo domain)
    {
        return @$"{workspacePath}\{domain.FullyQualifiedName}";
    }

    public string GetDomainPath(
        string workspacePath,
        DomainInfo domain)
    {
        var domainDirectory = GetDomainDirectory(workspacePath, domain);
        return @$"{domainDirectory}\{domain.FullyQualifiedName}.sln";
    }

    public string GetProjectDirectory(
        string workspacePath,
        ProjectInfo project)
    {
        var domain = project.Domain.EnsureNotNull();
        var domainDirectory = GetDomainDirectory(workspacePath, domain);
        if (domain.Name == project.Name)
            return @$"{domainDirectory}\src\{domain.FullyQualifiedName}";

        return @$"{domainDirectory}\src\{domain.FullyQualifiedName}.{project.Name}";
    }

    public string GetProjectPath(
        string workspacePath,
        ProjectInfo project)
    {
        var domain = project.Domain.EnsureNotNull();
        var projectDirectory = GetProjectDirectory(workspacePath, project);
        if (domain.Name == project.Name)
            return @$"{projectDirectory}\{domain.Name}.csproj";

        return @$"{projectDirectory}\{domain.Name}.{project.Name}.csproj";
    }

    public Result TryCreateWorkspaceInfo(
        string workspacePath,
        string workspaceName)
    {
        if (TryGetWorkspaceInfo(workspacePath).HasItem(out _))
            return ErrorResult.FromMessages(("Workspace {workspaceName} already exists.", workspaceName));

        var workspaceInfo = new WorkspaceInfo { Name = workspaceName };
        return TrySaveWorkspaceInfo(workspacePath, workspaceInfo);
    }

    public Result TryAddDomainInfo(
        string workspacePath,
        DomainInfo domain)
    {
        if (TryGetWorkspaceInfo(workspacePath).HasNoItem(out var workspaceInfo, out var messages))
            return ErrorResult.FromMessages([.. messages]);

        if (workspaceInfo.Domains.Any(x => x.Name == domain.Name))
            return ErrorResult.FromMessages(("Domain {domainName} already exists.", domain.Name));

        workspaceInfo.Domains = [.. workspaceInfo.Domains, domain];
        return TrySaveWorkspaceInfo(workspacePath, workspaceInfo);
    }

    public Result TryAddProjectInfo(
        string workspacePath,
        string domainName,
        ProjectInfo project)
    {
        if (TryGetWorkspaceInfo(workspacePath).HasNoItem(out var workspaceInfo, out var messages))
            return ErrorResult.FromMessages(messages);

        var domain = workspaceInfo.Domains.FirstOrDefault(x => x.Name == domainName);
        if (domain is null)
            return ErrorResult.FromMessages(("Unable to find domain {domainName}.", domainName));

        if (domain.Projects.Any(x => x.Name == project.Name))
            return ErrorResult.FromMessages(("Project {projectName} already exists on domain {domainName}.", project.Name, domain.Name));

        domain.Projects = [.. domain.Projects, project];
        return TrySaveWorkspaceInfo(workspacePath, workspaceInfo);
    }

    public Result TryAddPackageInfo(
        string workspacePath,
        string domainName,
        string projectName,
        PackageInfo package)
    {
        if (TryGetWorkspaceInfo(workspacePath).HasNoItem(out var workspaceInfo, out var messages))
            return ErrorResult.FromMessages(messages);

        var domain = workspaceInfo.Domains.FirstOrDefault(x => x.Name == domainName);
        if (domain is null)
            return ErrorResult.FromMessages(("Unable to find domain {domainName}.", domainName));

        var project = domain.Projects.FirstOrDefault(x => x.Name == projectName);
        if (project is null)
            return ErrorResult.FromMessages(("Unable to find project {projectName}.", projectName));

        if (project.PackageReferences.Any(x => x.Name == package.Name))
            return ErrorResult.FromMessages(("Package {packageName} already exists on project {projectName} on domain {domainName}.", package.Name, project.Name, domain.Name));

        project.PackageReferences = [.. project.PackageReferences, package];
        return TrySaveWorkspaceInfo(workspacePath, workspaceInfo);
    }

    private Result TrySaveWorkspaceInfo(
        string workspacePath,
        WorkspaceInfo workspaceInfo)
    {
        var workspaceInfoPath = @$"{workspacePath}\.workspace";
        _memoryCache.Set(workspaceInfoPath, workspaceInfo);
        return _fileSystemService.TryWriteFile(workspaceInfoPath, workspaceInfo);
    }

    public Result<WorkspaceInfo> TryGetWorkspaceInfo(
        string workspacePath)
    {
        var workspaceInfoPath = @$"{workspacePath}\.workspace";
        if (_memoryCache.TryGetValue<WorkspaceInfo>(workspaceInfoPath, out var workspaceInfo))
            return workspaceInfo.EnsureNotNull();

        var result = _fileSystemService.TryReadFile<WorkspaceInfo>(workspaceInfoPath);
        if (result.HasNoItem(out _, out var messages))
            return ErrorResult.FromMessages([.. messages]);

        using var entry = _memoryCache.CreateEntry(workspaceInfoPath);
        entry.Value = result;
        return result;
    }
}
