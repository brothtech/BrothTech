using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;
using BrothTech.DevKit.WorkspaceManagement.Services;
using BrothTech.DevKit.WorkspaceManagement.Workspaces.Services;
using BrothTech.Infrastructure.DependencyInjection;
using System.Text;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;

[ServiceDescriptor<ICommandHandler<ProjectAddCommand, ProjectAddCommandResult>, ProjectAddCommandHandler>]
public class ProjectAddCommandHandler(
    IFileSystemService fileSystemService,
    IWorkspaceManagementService workspaceService,
    IDotNetService dotNetService,
    IWorkspaceInfoService workspaceInfoService) :
    ICommandHandler<ProjectAddCommand, ProjectAddCommandResult>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IWorkspaceManagementService _workspaceService = workspaceService.EnsureNotNull();
    private readonly IDotNetService _dotNetService = dotNetService.EnsureNotNull();
    private readonly IWorkspaceInfoService _workspaceInfoService = workspaceInfoService.EnsureNotNull();

    public int Priority => 0;

    public async Task<Result> TryHandleAsync(
        ProjectAddCommandResult commandResult, 
        CancellationToken token)
    {
        var (workspace, project) = (null as WorkspaceInfo, null as ProjectInfo);
        var domainName = commandResult.DomainName ?? commandResult.Name;
        return TryGetWorkspacePath(commandResult.WorkspacePath).OutWithItem(out var workspacePath) &&
               _workspaceInfoService.TryGetWorkspaceInfo(workspacePath).OutWithItem(out workspace) &&
               TryCreateProjectInfo(commandResult, workspacePath, domainName).OutWithItem(out project) &&
               await TryCreateProjectAsync(commandResult, workspacePath, domainName, token) &&
               await TryAddToRootAndDomainSolution(workspacePath, workspace!, project!, token) &&
               await TryAddProjectReferencesAsync(workspacePath, workspace!, project!, token);
    }

    private Result<string> TryGetWorkspacePath(
        string? workspacePath)
    {
        if (workspacePath is not null)
            return workspacePath;

        return _workspaceService.TryGetWorkspaceRootPath();
    }

    private Result<ProjectInfo> TryCreateProjectInfo(
        ProjectAddCommandResult commandResult,
        string workspacePath,
        string domainName)
    {
        var project = new ProjectInfo
        {
            DomainName = domainName,
            Name = commandResult.Name,
            AssemblyName = "hi",
            ExposureType = commandResult.ExposureType.Value
        };
        var result = _workspaceInfoService.TryAddProjectInfo(workspacePath, domainName, project);
        if (result.IsSuccessful is false)
            return ErrorResult.FromMessages(result.Messages);

        return project;
    }
    
    private async Task<Result> TryCreateProjectAsync(
        ProjectAddCommandResult commandResult,
        string workspacePath,
        string domainName,
        CancellationToken token)
    {
        var projectDirectory = @$"{workspacePath}\{domainName}\src\{commandResult.Name}";
        var template = commandResult.Template ?? DotNetProjectTemplate.ClassLib;
        _fileSystemService.EnsureDirectoryExists(projectDirectory);
        return await _dotNetService.TryCreateProject(commandResult.Name, template, projectDirectory, token);
    }

    private async Task<Result> TryAddToRootAndDomainSolution(
        string workspacePath,
        WorkspaceInfo workspace,
        ProjectInfo project,
        CancellationToken token)
    {
        var rootSolutionPath = @$"{workspacePath}\{workspace.Name}.Root.sln";
        var domainSolutionPath = @$"{workspacePath}\{project.DomainName}\{project.DomainName}.sln";
        var projectPath = @$"{workspacePath}\{project.DomainName}\src\{project.Name}\{project.Name}.csproj";
        var solutionFolder = GetSolutionFolder(workspace, project);
        return await _dotNetService.TryAddProjectToSolution(rootSolutionPath, projectPath, solutionFolder, token) &&
               await _dotNetService.TryAddProjectToSolution(domainSolutionPath, projectPath, "src", token);
    }

    private string GetSolutionFolder(
        WorkspaceInfo workspace,
        ProjectInfo project)
    {
        var domain = workspace.Domains.First(x => x.Name == project.DomainName);
        var domainNames = new List<string> { domain.Name };
        while (domain.ParentDomainName is not null)
        {
            domainNames.Add(domain.ParentDomainName);
            domain = workspace.Domains.First(x => x.Name == domain.ParentDomainName);
        }
        
        domainNames.Add("src");
        domainNames.Reverse();
        return string.Join('\\', domainNames);
    }

    private async Task<Result> TryAddProjectReferencesAsync(
        string workspacePath,
        WorkspaceInfo workspace,
        ProjectInfo project,
        CancellationToken token)
    {
        var domain = workspace.Domains.First(x => x.Name == project.DomainName);
        var aggregateResult = Result.Success;
        foreach (var targetDomain in workspace.Domains)
        {
            var relationType = GetRelationType(workspace, domain, targetDomain);
            if (relationType is DomainRelationType.None)
                continue;

            aggregateResult &= await TryAddProjectReferencesAsync(workspacePath, project, targetDomain, relationType, token);
            if (aggregateResult.IsSuccessful is false)
                return aggregateResult;
        }

        return aggregateResult;
    }

    private DomainRelationType GetRelationType(
        WorkspaceInfo workspace,
        DomainInfo domain,
        DomainInfo targetDomain)
    {
        if (domain.Name == targetDomain.Name)
            return DomainRelationType.IntraDomainSibling;

        if (domain.ParentDomainName == targetDomain.ParentDomainName)
            return DomainRelationType.InterDomainSibling;

        if (IsDescendentDomain(workspace, domain, targetDomain))
            return DomainRelationType.Descendent;

        if (IsDescendentDomain(workspace, targetDomain, domain))
            return DomainRelationType.Ancestor;

        return DomainRelationType.None;
    }

    private bool IsDescendentDomain(
        WorkspaceInfo workspace,
        DomainInfo domain,
        DomainInfo targetDomain)
    {
        while (domain.ParentDomainName is not null)
        {
            if (domain.ParentDomainName == targetDomain.Name)
                return true;

            domain = workspace.Domains.First(x => x.Name == domain.Name);
        }

        return false;
    }

    private async Task<Result> TryAddProjectReferencesAsync(
        string workspacePath,
        ProjectInfo project,
        DomainInfo targetDomain,
        DomainRelationType relationType,
        CancellationToken token)
    {
        var aggregateResult = Result.Success;
        foreach (var targetProject in targetDomain.Projects)
        {
            if (relationType is DomainRelationType.IntraDomainSibling && project.Name == targetProject.Name)
                continue;

            aggregateResult &= await TryAddProjectReferencesAsync(workspacePath, project, targetProject, relationType, token);
            if (aggregateResult.IsSuccessful is false)
                return aggregateResult;
        }

        return aggregateResult;
    }

    private async Task<Result> TryAddProjectReferencesAsync(
        string workspacePath,
        ProjectInfo project,
        ProjectInfo targetProject,
        DomainRelationType relationType,
        CancellationToken token)
    {
        var projectPath = @$"{workspacePath}\{project.DomainName}\src\{project.Name}\{project.Name}.csproj";
        var targetProjectPath = @$"{workspacePath}\{targetProject.DomainName}\src\{targetProject.Name}\{targetProject.Name}.csproj";

        var aggregateResult = Result.Success;
        if (project.ExposureType.CanDependOn(targetProject.ExposureType, relationType))
            aggregateResult &= await _dotNetService.TryAddProjectReferenceAsync(projectPath, targetProjectPath, token);

        if (aggregateResult.IsSuccessful && project.ExposureType.IsVisibleTo(targetProject.ExposureType, relationType))
            aggregateResult &= await _dotNetService.TryAddProjectReferenceAsync(targetProjectPath, projectPath, token);

        return aggregateResult;
    }

    public bool ShouldInvokeNewCommands(
        ProjectAddCommandResult commandResult)
    {
        return false;
    }

    public IEnumerable<string[]> GetNewCommandsArgs(
        ProjectAddCommandResult commandResult)
    {
        return [];
    }
}