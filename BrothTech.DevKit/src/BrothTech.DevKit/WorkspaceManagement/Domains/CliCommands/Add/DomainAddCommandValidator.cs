using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Contracts.Results;
using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;
using BrothTech.DevKit.WorkspaceManagement.Services;
using BrothTech.DevKit.WorkspaceManagement.Workspaces.Services;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands.Add;

public class DomainAddCommandValidator(
    IWorkspaceManagementService workspaceService,
    IWorkspaceInfoService workspaceInfoService) :
    ICliCommandValidator<DomainAddCliCommand, IDomainAddCliCommandResult>
{
    private readonly IWorkspaceManagementService _workspaceService = workspaceService.EnsureNotNull();  
    private readonly IWorkspaceInfoService _workspaceInfoService = workspaceInfoService.EnsureNotNull();

    public async Task<Result> ValidateAsync(
        IDomainAddCliCommandResult command, 
        CancellationToken token = default)
    {
        if (command.DomainReferences is { Length: > 0 })
            return Result.Success;

        if (_workspaceService.TryGetWorkspaceRootPath().HasItem(out var workspacePath, out var messages) is false ||
            _workspaceInfoService.TryGetWorkspaceInfo(workspacePath).HasItem(out var workspace, out messages) is false)
            return ErrorResult.FromMessages(messages);

        foreach (var targetDomain in workspace.Domains.Where(x => command.DomainReferences.Contains(x.Name)))
        {
            if (targetDomain.ParentDomainName != command.ParentDomainName)
                return ErrorResult.FromMessages(("Domain {domainName} and target domain {targetDomainName} are not siblings.", command.Name, targetDomain.Name));

            if (targetDomain.DomainReferences.Contains(command.Name))
                return ErrorResult.FromMessages(("Domain {domainName} cannot reference {targetDomainName} as it is already referenced by it.", command.Name, targetDomain.Name));
        }

        return Result.Success;
    }
}
