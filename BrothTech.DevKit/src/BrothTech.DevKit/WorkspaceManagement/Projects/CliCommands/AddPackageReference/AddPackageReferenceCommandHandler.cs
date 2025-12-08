using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Services;
using BrothTech.DevKit.WorkspaceManagement.Workspaces.Services;
using BrothTech.Infrastructure.DependencyInjection;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.AddPackageReference;

[ServiceDescriptor<ICliCommandHandler<AddPackageReferenceCommand, IAddPackageReferenceCommandResult>, AddPackageReferenceCommandHandler>]
public class AddPackageReferenceCommandHandler(
    IFileSystemService fileSystemService,
    IWorkspaceManagementService workspaceService,
    IDotNetService dotNetService,
    IWorkspaceInfoService workspaceInfoService) :
    ICliCommandHandler<AddPackageReferenceCommand, IAddPackageReferenceCommandResult>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IWorkspaceManagementService _workspaceService = workspaceService.EnsureNotNull();
    private readonly IDotNetService _dotNetService = dotNetService.EnsureNotNull();
    private readonly IWorkspaceInfoService _workspaceInfoService = workspaceInfoService.EnsureNotNull();

    public int Priority => 0;

    public async Task<Result> TryHandleAsync(
        IAddPackageReferenceCommandResult commandResult, 
        CancellationToken token)
    {
        var workspace = null as WorkspaceInfo;
        var packageReference = new PackageInfo(commandResult.PackageName, commandResult.PackageVersion);
        return TryGetWorkspacePath(commandResult).OutWithItem(out var workspacePath) &&
               TryAddPackageInfo(commandResult, workspacePath) &&
               _workspaceInfoService.TryGetWorkspaceInfo(workspacePath).OutWithItem(out workspace) &&
               await TryAddPackageReferenceAsync(commandResult, workspacePath, workspace!, token);
    }

    private Result<string> TryGetWorkspacePath(
        IAddPackageReferenceCommandResult commandResult)
    {
        if (commandResult.WorkspacePath is not null)
            return commandResult.WorkspacePath;

        if (_workspaceService.TryGetWorkspaceRootPath().HasItem(out var workspacePath, out var messages))
            return commandResult.WorkspacePath = workspacePath;

        return ErrorResult.FromMessages(messages);
    }

    private Result TryAddPackageInfo(
        IAddPackageReferenceCommandResult commandResult,
        string workspacePath)
    {
        var package = new PackageInfo(commandResult.PackageName, commandResult.PackageVersion);
        return _workspaceInfoService.TryAddPackageInfo(
            workspacePath: workspacePath,
            domainName: commandResult.DomainName,
            projectName: commandResult.ProjectName,
            package: package);
    }

    private async Task<Result> TryAddPackageReferenceAsync(
        IAddPackageReferenceCommandResult commandResult,
        string workspacePath,
        WorkspaceInfo workspace,
        CancellationToken token)
    {
        if (workspace.Domains.FirstOrDefault(x => x.Name == commandResult.DomainName) is not { } domain)
            return ErrorResult.FromMessages(("Domain {domainName} does not exist.", commandResult.DomainName));

        if (domain.Projects.FirstOrDefault(x => x.Name == commandResult.ProjectName) is not { } project)
            return ErrorResult.FromMessages(("Domain {projectName} does not exist.", commandResult.ProjectName));

        var projectPath = GetProjectPath(workspacePath, project);
        return await _dotNetService.TryAddPackageReference(
            projectPath: projectPath,
            packageName: commandResult.PackageName,
            packageVersion: commandResult.PackageVersion,
            token: token);
    }

    private string GetSolutionDirectory(
        string workspacePath,
        string fullyQualifiedDomainName)
    {
        return @$"{workspacePath}\{fullyQualifiedDomainName}";
    }

    private string GetSolutionPath(
        string workspacePath,
        DomainInfo domain)
    {
        return GetSolutionPath(workspacePath, domain.FullyQualifiedName);
    }

    private string GetSolutionPath(
        string workspacePath,
        string fullyQualifiedDomainName)
    {
        var solutionDirectory = GetSolutionDirectory(workspacePath, fullyQualifiedDomainName);
        return @$"{solutionDirectory}\{fullyQualifiedDomainName}.sln";
    }

    private string GetProjectDirectory(
        string workspacePath,
        ProjectInfo project)
    {
        var domain = project.Domain.EnsureNotNull();
        var solutionDirectory = GetSolutionDirectory(workspacePath, domain.FullyQualifiedName);
        if (domain.Name == project.Name)
            return @$"{solutionDirectory}\src\{domain.FullyQualifiedName}";

        return @$"{solutionDirectory}\src\{domain.FullyQualifiedName}.{project.Name}";
    }

    private string GetProjectPath(
        string workspacePath,
        ProjectInfo project)
    {
        var domain = project.Domain.EnsureNotNull();
        var projectDirectory = GetProjectDirectory(workspacePath, project);
        if (domain.Name == project.Name)
            return @$"{projectDirectory}\{domain.Name}.csproj";

        return @$"{projectDirectory}\{domain.Name}.{project.Name}.csproj";
    }

    public bool ShouldInvokeNewCommands(
        IAddPackageReferenceCommandResult commandResult)
    {
        return false;
    }

    public IEnumerable<string[]> GetNewCommandsArgs(
        IAddPackageReferenceCommandResult commandResult)
    {
        yield break;
    }
}