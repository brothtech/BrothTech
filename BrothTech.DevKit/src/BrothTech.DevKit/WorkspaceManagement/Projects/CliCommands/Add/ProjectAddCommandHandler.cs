using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands.Add;
using BrothTech.DevKit.WorkspaceManagement.Services;
using BrothTech.DevKit.WorkspaceManagement.Workspaces.Services;
using BrothTech.Infrastructure.DependencyInjection;
using System.Xml.Linq;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

[ServiceDescriptor<ICliCommandHandler<ProjectAddCliCommand, IProjectAddCliCommandResult>, ProjectAddCliCommandHandler>]
public class ProjectAddCliCommandHandler(
    IFileSystemService fileSystemService,
    IWorkspaceManagementService workspaceService,
    IDotNetService dotNetService,
    IWorkspaceInfoService workspaceInfoService) :
    ICliCommandHandler<ProjectAddCliCommand, IProjectAddCliCommandResult>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IWorkspaceManagementService _workspaceService = workspaceService.EnsureNotNull();
    private readonly IDotNetService _dotNetService = dotNetService.EnsureNotNull();
    private readonly IWorkspaceInfoService _workspaceInfoService = workspaceInfoService.EnsureNotNull();

    public int Priority => 0;

    public async Task<Result> TryHandleAsync(
        IProjectAddCliCommandResult commandResult, 
        CancellationToken token)
    {
        var (workspace, project) = (null as WorkspaceInfo, null as ProjectInfo);
        return TryGetWorkspacePath(commandResult).OutWithItem(out var workspacePath) &&
               _workspaceInfoService.TryGetWorkspaceInfo(workspacePath).OutWithItem(out workspace) &&
               TryCreateProjectInfo(commandResult, workspacePath, workspace.EnsureNotNull()).OutWithItem(out project) &&
               await TryCreateProjectAsync(commandResult, workspacePath, workspace.EnsureNotNull(), project.EnsureNotNull(), token) &&
               await TryAddToRootAndDomainSolution(workspacePath, workspace.EnsureNotNull(), project.EnsureNotNull(), token) &&
               await TryAddProjectReferencesAsync(workspacePath, workspace.EnsureNotNull(), project.EnsureNotNull(), token);
    }

    private Result<string> TryGetWorkspacePath(
        IProjectAddCliCommandResult commandResult)
    {
        if (commandResult.WorkspacePath is not null)
            return commandResult.WorkspacePath;

        if (_workspaceService.TryGetWorkspaceRootPath().HasItem(out var workspacePath, out var messages))
            return commandResult.WorkspacePath = workspacePath;

        return ErrorResult.FromMessages(messages);
    }

    private Result<ProjectInfo> TryCreateProjectInfo(
        IProjectAddCliCommandResult commandResult,
        string workspacePath,
        WorkspaceInfo workspace)
    {
        commandResult.DomainName = commandResult.DomainName ?? commandResult.Name;
        var domain = workspace.Domains.First(x => x.Name == commandResult.DomainName);
        var project = new ProjectInfo
        {
            DomainName = commandResult.DomainName,
            Name = commandResult.Name,
            AssemblyName = domain.Name != commandResult.Name ?
                $"{domain.FullyQualifiedName}.{commandResult.Name}" :
                domain.FullyQualifiedName,
            ExposureType = commandResult.ExposureType.Value
        };
        var result = _workspaceInfoService.TryAddProjectInfo(workspacePath, commandResult.DomainName, project);
        if (result.IsSuccessful is false)
            return ErrorResult.FromMessages(result.Messages);

        return project;
    }
    
    private async Task<Result> TryCreateProjectAsync(
        IProjectAddCliCommandResult commandResult,
        string workspacePath,
        WorkspaceInfo workspace,
        ProjectInfo project,
        CancellationToken token)
    {
        commandResult.Template ??= DotNetProjectTemplate.ClassLib;
        project.Domain ??= workspace.Domains.First(x => x.Name == project.DomainName);
        var directory = GetProjectDirectory(workspacePath, project);
        var path = GetProjectPath(workspacePath, project);
        var name = project.DomainName != project.Name ?
            $"{project.DomainName}.{project.Name}" :
            project.Name;
        var rootNamespace = project.Domain.Name != project.Name ?
            $"{project.Domain.FullyQualifiedName}.{project.Name}" :
            project.Domain.FullyQualifiedName;
        var template = commandResult.Template.Value;
        _fileSystemService.EnsureDirectoryExists(directory);
        return await _dotNetService.TryCreateProject(name, template, directory, token) &&
               await _dotNetService.TrySetPropertiesAsync(
                   projectPath: path,
                   token: token,
                   new("RootNamespace", rootNamespace),
                   new("AssemblyName", rootNamespace));
    }

    private async Task<Result> TryAddToRootAndDomainSolution(
        string workspacePath,
        WorkspaceInfo workspace,
        ProjectInfo project,
        CancellationToken token)
    {
        project.Domain ??= workspace.Domains.First(x => x.Name == project.DomainName);
        var rootSolutionPath = @$"{workspacePath}\{workspace.Name}.Root.sln";
        var domainSolutionPath = GetSolutionPath(workspacePath, project.Domain);
        var projectPath = GetProjectPath(workspacePath, project);
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
        project.Domain ??= workspace.Domains.First(x => x.Name == project.DomainName);
        var aggregateResult = Result.Success;
        foreach (var targetDomain in workspace.Domains)
        {
            var relationType = GetRelationType(workspace, project.Domain, targetDomain);
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
            return GetInterDomainRelationType(domain, targetDomain);

        if (IsDescendentDomain(workspace, domain, targetDomain))
            return DomainRelationType.Descendent;

        if (IsDescendentDomain(workspace, targetDomain, domain))
            return DomainRelationType.Ancestor;

        return DomainRelationType.None;
    }

    private DomainRelationType GetInterDomainRelationType(
        DomainInfo domain,
        DomainInfo targetDomain)
    {
        if (domain.DomainReferences.Any(x => x == targetDomain.Name))
            return DomainRelationType.InterDomainReferencer;

        if (targetDomain.DomainReferences.Any(x => x == domain.Name))
            return DomainRelationType.InterDomainReferencee;

        return DomainRelationType.InterDomainSibling;
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
            targetProject.Domain ??= targetDomain;
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
        var projectPath = GetProjectPath(workspacePath, project);
        var targetProjectPath = GetProjectPath(workspacePath, targetProject);

        var aggregateResult = Result.Success;
        if (project.ExposureType.CanDependOn(targetProject.ExposureType, relationType))
            aggregateResult &= await TryAddProjectReferenceAsync(project, targetProject, projectPath, targetProjectPath, token);

        if (aggregateResult.IsSuccessful && project.ExposureType.IsVisibleTo(targetProject.ExposureType, relationType))
            aggregateResult &= await TryAddProjectReferenceAsync(targetProject, project, targetProjectPath, projectPath, token);

        return aggregateResult;
    }

    private async Task<Result> TryAddProjectReferenceAsync(
        ProjectInfo project,
        ProjectInfo targetProject,
        string projectPath,
        string targetProjectPath,
        CancellationToken token)
    {
        var result = await _dotNetService.TryAddProjectReferenceAsync(projectPath, targetProjectPath, token);
        if (project.ExposureType is not ProjectExposureType.Internal)
            return result;

        return result && await _dotNetService.TryAddInternalVisiblityAsync(projectPath, targetProject.AssemblyName, token);
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
        IProjectAddCliCommandResult commandResult)
    {
        return false;
    }

    public IEnumerable<string[]> GetNewCommandsArgs(
        IProjectAddCliCommandResult commandResult)
    {
        return [];
    }
}

[Flags]
public enum DomainRelationType
{
    None = 0,
    Ancestor,
    Descendent,
    IntraDomainSibling,
    InterDomainSibling,
    InterDomainReferencer,
    InterDomainReferencee
}