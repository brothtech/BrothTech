using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands;
using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;
using BrothTech.DevKit.WorkspaceManagement.Services;
using BrothTech.DevKit.WorkspaceManagement.Workspaces.Services;
using BrothTech.Infrastructure.DependencyInjection;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands.Add;

[ServiceDescriptor<ICliCommandHandler<DomainAddCliCommand, IDomainAddCliCommandResult>, DomainAddCliCommandHandler>]
public class DomainAddCliCommandHandler(
    IFileSystemService fileSystemService,
    IWorkspaceManagementService workspaceService,
    IDotNetService dotNetService,
    IWorkspaceInfoService workspaceInfoService) :
    ICliCommandHandler<DomainAddCliCommand, IDomainAddCliCommandResult>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IWorkspaceManagementService _workspaceService = workspaceService.EnsureNotNull();
    private readonly IDotNetService _dotNetService = dotNetService.EnsureNotNull();
    private readonly IWorkspaceInfoService _workspaceInfoService = workspaceInfoService.EnsureNotNull();

    public int Priority => 0;

    public async Task<Result> TryHandleAsync(
        IDomainAddCliCommandResult commandResult, 
        CancellationToken token)
    {
        var fullyQualifiedName = null as string;
        return TryGetWorkspacePath(commandResult).OutWithItem(out var workspacePath) &&
               TryGetFullyQualifiedName(commandResult, workspacePath).OutWithItem(out fullyQualifiedName) &&
               await TryCreateDomainSolutionAsync(workspacePath, fullyQualifiedName!, token) &&
               TryCreateDomainInfo(workspacePath, commandResult.Name, commandResult.ParentDomainName, fullyQualifiedName!);
    }

    private Result<string> TryGetFullyQualifiedName(
        IDomainAddCliCommandResult commandResult,
        string workspacePath)
    {
        if (commandResult.FullyQualifiedName is not null)
            return commandResult.FullyQualifiedName;

        if (commandResult.ParentDomainName is null)
            return commandResult.FullyQualifiedName = commandResult.Name;

        var result = _workspaceInfoService.TryGetWorkspaceInfo(workspacePath);
        if (result.HasItem(out var workspace, out var messages) is false)
            return ErrorResult.FromMessages(messages);

        var domain = workspace.Domains.First(x => x.Name == commandResult.ParentDomainName);
        return commandResult.FullyQualifiedName = $"{domain.FullyQualifiedName}.{commandResult.Name}";
    }

    private Result<string> TryGetWorkspacePath(
        IDomainAddCliCommandResult commandResult)
    {
        if (commandResult.WorkspacePath is not null)
            return commandResult.WorkspacePath;

        if (_workspaceService.TryGetWorkspaceRootPath().HasItem(out var workspacePath, out var messages))
            return commandResult.WorkspacePath = workspacePath;

        return ErrorResult.FromMessages(messages);
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
        IDomainAddCliCommandResult commandResult)
    {
        return true;
    }

    public IEnumerable<string[]> GetNewCommandsArgs(
        IDomainAddCliCommandResult commandResult)
    {
        yield return [.. GetProjectAddCliCommandArgs(commandResult, commandResult.Name)];

        if (commandResult.ShouldAddSharedProject is true)
            yield return [.. GetProjectAddCliCommandArgs(
                commandResult: commandResult, 
                projectName: "Shared", 
                exposureType: ProjectExposureType.Shared,
                template: DotNetProjectTemplate.ClassLib)];

        if (commandResult.ShouldAddSandboxProject is true)
            yield return [.. GetProjectAddCliCommandArgs(
                commandResult: commandResult, 
                projectName: "Sandbox", 
                exposureType: ProjectExposureType.Sandbox,
                template: DotNetProjectTemplate.Console)];

        if (commandResult.ShouldAddInternalProject is true)
            yield return [.. GetProjectAddCliCommandArgs(
                commandResult: commandResult,
                projectName: "Internal",
                exposureType: ProjectExposureType.Internal,
                template: DotNetProjectTemplate.ClassLib)];
    }

    private IEnumerable<string> GetProjectAddCliCommandArgs(
        IDomainAddCliCommandResult commandResult,
        string projectName,
        ProjectExposureType? exposureType = null,
        DotNetProjectTemplate? template = null)
    {
        yield return nameof(ProjectCliCommand);
        yield return nameof(ProjectAddCliCommand);
        yield return projectName;
        yield return exposureType?.ToString() ?? commandResult.ExposureType.Value.ToString();
        yield return $"--{nameof(IHaveDomainNameOption.DomainName)}";
        yield return commandResult.Name;

        if (commandResult.WorkspacePath.IsNullOrWhiteSpace() is false)
        {
            yield return $"--{nameof(IHaveWorkspacePathOption.WorkspacePath)}";
            yield return commandResult.WorkspacePath;
        }

        if ((template ?? commandResult.Template) is { } templateValue and not DotNetProjectTemplate.None)
        {
            yield return $"--{nameof(IBaseProjectAddCliCommand.Template)}";
            yield return templateValue.ToString();
        }

        if (commandResult.FullyQualifiedName.IsNullOrWhiteSpace() is false)
        {
            yield return $"--{nameof(IBaseProjectAddCliCommand.FullyQualifiedName)}";
            yield return commandResult.FullyQualifiedName;
        }
    }
}
