using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
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
        if (TryGetWorkspacePath(commandResult.WorkspacePath).HasNoItem(out var workspacePath, out var messages))
            return ErrorResult.FromMessages([.. messages]);

        return await TryCreateDomainSolutionAsync(workspacePath, commandResult.Name, token) &&
               TryCreateDomainInfo(workspacePath, commandResult.Name, commandResult.ParentDomainName);
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
        string domainName,
        CancellationToken token)
    {
        var path = @$"{workspacePath}\{domainName}";
        _fileSystemService.EnsureDirectoryExists(path);
        return await _dotNetService.TryCreateSolutionAsync(domainName, path, token);
    }

    private Result TryCreateDomainInfo(
        string workspacePath,
        string domainName,
        string? parentDomainName)
    {
        var domain = new DomainInfo
        {
            ParentDomainName = parentDomainName,
            Name = domainName
        };
        return _workspaceInfoService.TryAddDomainInfo(workspacePath, domain);
    }

    public bool ShouldInvokeNewCommand(
        DomainAddCommandResult commandResult)
    {
        return true;
    }

    public IEnumerable<string> GetNewCommandArgs(
        DomainAddCommandResult commandResult)
    {
        yield return nameof(ProjectAddCommand);
        yield return commandResult.Name;
        yield return commandResult.ExposureType.Value.ToString();
        if (commandResult.WorkspacePath.IsNullOrWhiteSpace() is false)
        {
            yield return $"--{nameof(ProjectAddCommand.WorkspacePath)}";
            yield return commandResult.WorkspacePath;
        }

        if (commandResult.DomainName.IsNullOrWhiteSpace() is false)
        {
            yield return $"--{nameof(ProjectAddCommand.DomainName)}";
            yield return commandResult.DomainName;
        }

        if (commandResult.Template is not null and not DotNetProjectTemplate.None)
        {
            yield return $"--{nameof(ProjectAddCommand.Template)}";
            yield return commandResult.Template.Value.ToString();
        }
    }
}
