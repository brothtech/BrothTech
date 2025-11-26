using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.Infrastructure.Files;
using BrothTech.DevKit.WorkspaceManagement.Services;
using BrothTech.Infrastructure.DependencyInjection;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Workspace.Initialize;

[ServiceDescriptor<ICommandHandler<WorkspaceInitializeCommand, WorkspaceInitializeCommandResult>, WorkspaceInitializeCommandHandler>]
public class WorkspaceInitializeCommandHandler(
    IFileSystemService fileSystemService,
    IWorkspaceManagementService workspaceService,
    IDotNetService dotNetService) :
    ICommandHandler<WorkspaceInitializeCommand, WorkspaceInitializeCommandResult>
{
    private readonly IFileSystemService _fileSystemService = fileSystemService.EnsureNotNull();
    private readonly IWorkspaceManagementService _workspaceService = workspaceService.EnsureNotNull();
    private readonly IDotNetService _dotNetService = dotNetService.EnsureNotNull();

    public async Task<Result> TryHandleAsync(
        WorkspaceInitializeCommandResult commandResult, 
        CancellationToken token)
    {
        var workspaceName = commandResult.Name;
        var template = 
        var exposureType = commandResult.Exposure ?? ProjectExposureType.Public;
        _fileSystemService.EnsureDirectoryExists(@$".\{commandResult.Name}");
        return TryCreateWorkspaceInfo(commandResult.Name) &&
               await EnsureSolutionExistsAsync(commandResult.Name, token);
    }

    private Result TryCreateWorkspaceInfo(
        string workspaceName,
        ProjectExposureType exposureType)
    {
        var path = @$".\{workspaceName}\.workspace";
        if (_fileSystemService.FileExists(path))
            return ErrorResult.FromMessages(("Workspace {workspaceName} already exists", workspaceName));

        var primaryProject = new ProjectInfo
        {
            Name = workspaceName,
            ExposureType = exposureType,
        };
        var sharedProject = new ProjectInfo
        {
            Name = $"{workspaceName}.Shared",
            ExposureType = ProjectExposureType.Shared
        };
        var projects = new ProjectInfo[]
        {
            new ProjectInfo 
            { 
                Name = workspaceName,
                ExposureType = exposureType
            }
        };
        var workspaceInfo = new WorkspaceInfo
        {
            Name = workspaceName,
            Domains =
            [
                new DomainInfo
                {
                    Name = workspaceName,
                    Projects =
                    [
                        new ProjectInfo 
                        {
                            Name = workspaceName,
                            IsTopLevel
                        }
                    ]
                }
            ]
        };
        return _fileSystemService.TryWriteFile(path, workspaceInfo);
    }

    private async Task<Result> EnsureSolutionExistsAsync(
        string workspaceName,
        CancellationToken token)
    {
        var path = @$".\{workspaceName}";
        if (_fileSystemService.FileExists(@$"{path}\{workspaceName}.Root.sln"))
            return Result.Success;

        return await _dotNetService.TryCreateSolutionAsync(workspaceName, path, token);
    }
}