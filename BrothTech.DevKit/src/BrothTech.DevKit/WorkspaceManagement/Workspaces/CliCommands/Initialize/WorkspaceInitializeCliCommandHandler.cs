using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands;
using BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands.Add;
using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;
using BrothTech.DevKit.WorkspaceManagement.Services;
using BrothTech.DevKit.WorkspaceManagement.Workspaces.Services;
using BrothTech.Infrastructure.DependencyInjection;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.CliCommands.Initialize;

[ServiceDescriptor<ICliCommandHandler<WorkspaceInitializeCliCommand, IWorkspaceInitializeCliCommandResult>, WorkspaceInitializeCliCommandHandler>]
public class WorkspaceInitializeCliCommandHandler(
    IFileSystemService fileSystemService,
    IWorkspaceManagementService workspaceService,
    IDotNetService dotNetService,
    IWorkspaceInfoService workspaceInfoService) :
    ICliCommandHandler<WorkspaceInitializeCliCommand, IWorkspaceInitializeCliCommandResult>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IWorkspaceManagementService _workspaceService = workspaceService.EnsureNotNull();
    private readonly IDotNetService _dotNetService = dotNetService.EnsureNotNull();
    private readonly IWorkspaceInfoService _workspaceInfoService = workspaceInfoService.EnsureNotNull();

    public int Priority => 0;

    public async Task<Result> TryHandleAsync(
        IWorkspaceInitializeCliCommandResult commandResult, 
        CancellationToken token)
    {
        var workspacePath = commandResult.WorkspacePath ?? @$".\{commandResult.Name}";
        _fileSystemService.EnsureDirectoryExists(workspacePath);
        return _workspaceInfoService.TryCreateWorkspaceInfo(workspacePath, commandResult.Name) &&
               await _dotNetService.TryCreateSolutionAsync($"{commandResult.Name}.Root", workspacePath, token);
    }

    public bool ShouldInvokeNewCommands(
        IWorkspaceInitializeCliCommandResult commandResult)
    {
        return true;
    }

    public IEnumerable<string[]> GetNewCommandsArgs(
        IWorkspaceInitializeCliCommandResult commandResult)
    {
        yield return [.. GetDomainAddCliCommandArgs(commandResult)];
    }

    private IEnumerable<string> GetDomainAddCliCommandArgs(
        IWorkspaceInitializeCliCommandResult commandResult)
    {
        yield return nameof(DomainCliCommand);
        yield return nameof(DomainAddCliCommand);
        yield return commandResult.DomainName.IfNullOrWhiteSpace(commandResult.Name);
        yield return commandResult.ExposureType.Value.ToString();

        if (commandResult.WorkspacePath.IsNullOrWhiteSpace() is false)
        {
            yield return $"--{nameof(IHaveWorkspacePathOption.WorkspacePath)}";
            yield return commandResult.WorkspacePath;
        }

        if (commandResult.Template is not null and not DotNetProjectTemplate.None)
        {
            yield return $"--{nameof(IBaseDomainAddCliCommand.Template)}";
            yield return commandResult.Template.Value.ToString();
        }

        if (commandResult.ShouldAddSharedProject.Value)
            yield return $"--{nameof(IBaseDomainAddCliCommand.ShouldAddSharedProject)}";

        if (commandResult.ShouldAddSandboxProject.Value)
            yield return $"--{nameof(IBaseDomainAddCliCommand.ShouldAddSandboxProject)}";

        if (commandResult.ShouldAddInternalProject.Value)
            yield return $"--{nameof(IBaseDomainAddCliCommand.ShouldAddInternalProject)}";
    }
}