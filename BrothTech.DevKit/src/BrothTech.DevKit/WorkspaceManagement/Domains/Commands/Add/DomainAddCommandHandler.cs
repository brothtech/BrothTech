using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Projects.Commands;
using BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;
using BrothTech.DevKit.WorkspaceManagement.Services;
using BrothTech.DevKit.WorkspaceManagement.Workspaces.Services;
using BrothTech.Infrastructure.DependencyInjection;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.Commands.Add;

[ServiceDescriptor<ICommandHandler<DomainAddCommand, DomainAddCommandResult>, DomainAddCommandHandler>]
public class DomainAddCommandHandler(
    IFileSystemService fileSystemService,
    IWorkspaceManagementService workspaceService,
    IDotNetService dotNetService,
    IWorkspaceInfoService workspaceInfoService) :
    ICommandHandler<DomainAddCommand, DomainAddCommandResult>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IWorkspaceManagementService _workspaceService = workspaceService.EnsureNotNull();
    private readonly IDotNetService _dotNetService = dotNetService.EnsureNotNull();
    private readonly IWorkspaceInfoService _workspaceInfoService = workspaceInfoService.EnsureNotNull();

    public int Priority => 0;

    public async Task<Result> TryHandleAsync(
        DomainAddCommandResult commandResult, 
        CancellationToken token)
    {
        var fullyQualifiedName = null as string;
        return TryGetWorkspacePath(commandResult.WorkspacePath).OutWithItem(out var workspacePath) &&
               TryGetFullyQualifiedName(commandResult, workspacePath).OutWithItem(out fullyQualifiedName) &&
               await TryCreateDomainSolutionAsync(workspacePath, fullyQualifiedName!, token) &&
               TryCreateDomainInfo(workspacePath, commandResult.Name, commandResult.ParentDomainName, fullyQualifiedName!);
    }

    private Result<string> TryGetFullyQualifiedName(
        DomainAddCommandResult commandResult,
        string workspacePath)
    {
        if (commandResult.FullyQualifiedName is null)
            return ErrorResult.Failure;

        if (commandResult.ParentDomainName is null)
            return commandResult.FullyQualifiedName = commandResult.Name;

        var result = _workspaceInfoService.TryGetWorkspaceInfo(workspacePath);
        if (result.HasItem(out var workspace, out var messages) is false)
            return ErrorResult.FromMessages(messages);

        var domain = workspace.Domains.First(x => x.Name == commandResult.ParentDomainName);
        return commandResult.FullyQualifiedName = $"{domain.FullyQualifiedName}.{commandResult.Name}";
    }

    private Result<string> TryGetWorkspacePath(
        string? workspacePath)
    {
        if (workspacePath is not null)
            return workspacePath;

        return _workspaceService.TryGetWorkspaceRootPath();
    }

    private async Task<Result> TryCreateDomainSolutionAsync(
        string workspacePath,
        string fullyQualifiedName,
        CancellationToken token)
    {
        var path = @$"{workspacePath}\{fullyQualifiedName}";
        _fileSystemService.EnsureDirectoryExists(path);
        return await _dotNetService.TryCreateSolutionAsync(fullyQualifiedName, path, token);
    }

    private Result TryCreateDomainInfo(
        string workspacePath,
        string domainName,
        string? parentDomainName,
        string fullyQualifiedName)
    {
        var domain = new DomainInfo
        {
            ParentDomainName = parentDomainName,
            Name = domainName,
            FullyQualifiedName = fullyQualifiedName
        };
        return _workspaceInfoService.TryAddDomainInfo(workspacePath, domain);
    }

    public bool ShouldInvokeNewCommands(
        DomainAddCommandResult commandResult)
    {
        return true;
    }

    public IEnumerable<string[]> GetNewCommandsArgs(
        DomainAddCommandResult commandResult)
    {
        yield return [.. GetProjectAddCommandArgs(commandResult)];

        if (commandResult.ShouldAddSharedProject is true)
            yield return [.. GetProjectAddCommandArgs(
                commandResult: commandResult, 
                projectNameSuffix: ".Shared", 
                exposureType: ProjectExposureType.Shared,
                template: DotNetProjectTemplate.ClassLib)];

        if (commandResult.ShouldAddSandboxProject is true)
            yield return [.. GetProjectAddCommandArgs(
                commandResult: commandResult, 
                projectNameSuffix: ".Sandbox", 
                exposureType: ProjectExposureType.Sandbox,
                template: DotNetProjectTemplate.Console)];

        if (commandResult.ShouldAddInternalProject is true)
            yield return [.. GetProjectAddCommandArgs(
                commandResult: commandResult,
                projectNameSuffix: ".Internal",
                exposureType: ProjectExposureType.Internal,
                template: DotNetProjectTemplate.ClassLib)];
    }

    private IEnumerable<string> GetProjectAddCommandArgs(
        DomainAddCommandResult commandResult,
        string? projectNameSuffix = null,
        ProjectExposureType? exposureType = null,
        DotNetProjectTemplate? template = null)
    {
        yield return nameof(ProjectCommand);
        yield return nameof(ProjectAddCommand);
        yield return $"{commandResult.Name}{projectNameSuffix}";
        yield return exposureType?.ToString() ?? commandResult.ExposureType.Value.ToString();
        yield return $"--{nameof(ProjectAddCommand.DomainName)}";
        yield return commandResult.Name;

        if (commandResult.WorkspacePath.IsNullOrWhiteSpace() is false)
        {
            yield return $"--{nameof(ProjectAddCommand.WorkspacePath)}";
            yield return commandResult.WorkspacePath;
        }

        if ((template ?? commandResult.Template) is { } templateValue and not DotNetProjectTemplate.None)
        {
            yield return $"--{nameof(ProjectAddCommand.Template)}";
            yield return templateValue.ToString();
        }

        if (commandResult.FullyQualifiedName.IsNullOrWhiteSpace() is false)
        {
            yield return $"--{nameof(ProjectAddCommand.FullyQualifiedName)}";
            yield return commandResult.FullyQualifiedName;
        }
    }
}
