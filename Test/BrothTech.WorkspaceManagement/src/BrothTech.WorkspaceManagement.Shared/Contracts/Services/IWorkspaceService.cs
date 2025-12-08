using BrothTech.Shared.Contracts.Results;
using BrothTech.WorkspaceManagement.Shared.Contracts.Workspaces;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Services;

public interface IWorkspaceService
{
    Result<string> TryGetWorkspaceDirectory();

    string GetDomainDirectory(
        string workspacePath,
        DomainInfo domain);

    string GetDomainPath(
        string workspacePath,
        DomainInfo domain);

    string GetProjectDirectory(
        string workspacePath,
        ProjectInfo project);

    string GetProjectPath(
        string workspacePath,
        ProjectInfo project);

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

    Result TryAddPackageInfo(
        string workspacePath,
        string domainName,
        string projectName,
        PackageInfo package);

    Result<WorkspaceInfo> TryGetWorkspaceInfo(
        string workspacePath);
}

