using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Domains.Commands;
using BrothTech.DevKit.WorkspaceManagement.Domains.Commands.Add;
using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;
using BrothTech.DevKit.WorkspaceManagement.Services;
using BrothTech.DevKit.WorkspaceManagement.Workspaces.Services;
using BrothTech.Infrastructure.DependencyInjection;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.Commands.Initialize;

[ServiceDescriptor<ICommandHandler<WorkspaceInitializeCommand, WorkspaceInitializeCommandResult>, WorkspaceInitializeCommandHandler>]
public class WorkspaceInitializeCommandHandler(
    IFileSystemService fileSystemService,
    IWorkspaceManagementService workspaceService,
    IDotNetService dotNetService,
    IWorkspaceInfoService workspaceInfoService) :
    ICommandHandler<WorkspaceInitializeCommand, WorkspaceInitializeCommandResult>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IWorkspaceManagementService _workspaceService = workspaceService.EnsureNotNull();
    private readonly IDotNetService _dotNetService = dotNetService.EnsureNotNull();
    private readonly IWorkspaceInfoService _workspaceInfoService = workspaceInfoService.EnsureNotNull();

    public int Priority => 0;

    public async Task<Result> TryHandleAsync(
        WorkspaceInitializeCommandResult commandResult, 
        CancellationToken token)
    {
        var workspacePath = commandResult.WorkspacePath ?? @$".\{commandResult.Name}";
        _fileSystemService.EnsureDirectoryExists(workspacePath);
        return _workspaceInfoService.TryCreateWorkspaceInfo(workspacePath, commandResult.Name) &&
               await _dotNetService.TryCreateSolutionAsync($"{commandResult.Name}.Root", workspacePath, token);
    }

    public bool ShouldInvokeNewCommands(
        WorkspaceInitializeCommandResult commandResult)
    {
        return true;
    }

    public IEnumerable<string[]> GetNewCommandsArgs(
        WorkspaceInitializeCommandResult commandResult)
    {
        yield return [.. GetDomainAddCommandArgs(commandResult)];
    }

    private IEnumerable<string> GetDomainAddCommandArgs(
        WorkspaceInitializeCommandResult commandResult)
    {
        yield return nameof(DomainCommand);
        yield return nameof(DomainAddCommand);
        yield return commandResult.DomainName.IfNullOrWhiteSpace(commandResult.Name);
        yield return commandResult.ExposureType.Value.ToString();

        if (commandResult.WorkspacePath.IsNullOrWhiteSpace() is false)
        {
            yield return $"--{nameof(DomainAddCommand.WorkspacePath)}";
            yield return commandResult.WorkspacePath;
        }

        if (commandResult.Template is not null and not DotNetProjectTemplate.None)
        {
            yield return $"--{nameof(DomainAddCommand.Template)}";
            yield return commandResult.Template.Value.ToString();
        }

        if (commandResult.ShouldAddSharedProject.Value)
            yield return $"--{nameof(DomainAddCommand.ShouldAddSharedProject)}";

        if (commandResult.ShouldAddSandboxProject.Value)
            yield return $"--{nameof(DomainAddCommand.ShouldAddSandboxProject)}";

        if (commandResult.ShouldAddInternalProject.Value)
            yield return $"--{nameof(DomainAddCommand.ShouldAddInternalProject)}";
    }
}