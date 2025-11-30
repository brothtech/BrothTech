using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Services;
using Microsoft.Extensions.Caching.Memory;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.Services;

public interface IWorkspaceInfoService
{
    Result TryCreateWorkspaceInfo(
        string workspacePath,
        string workspaceName);

    Result TryAddDomainInfo(
        string workspacePath,
        DomainInfo domain);

    Result TryAddProjectInfo(
        string workspacePath,
        string domainName,
        ProjectInfo project);

    Result<WorkspaceInfo> TryGetWorkspaceInfo(
        string workspacePath);
}

public class WorkspaceInfoService(
    IMemoryCache memoryCache,
    IFileSystemService fileSystemService) :
    IWorkspaceInfoService
{
    private readonly IMemoryCache _memoryCache = memoryCache.EnsureNotNull();
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    
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
            return ErrorResult.FromMessages(
                ("Project {projectName} already exists on domain {domainName}.", project.Name, domain.Name));

        domain.Projects = [.. domain.Projects, project];
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
