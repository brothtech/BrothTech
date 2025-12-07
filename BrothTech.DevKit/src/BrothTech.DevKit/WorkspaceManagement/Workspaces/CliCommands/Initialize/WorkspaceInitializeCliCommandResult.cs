using BrothTech.Cli.Shared.CliCommands;
using BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands.Add;
using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.CliCommands.Initialize;

public interface IWorkspaceInitializeCliCommandResult :
    IBaseDomainAddCliCommandResult<WorkspaceInitializeCliCommand>,
    IHaveDomainNameOptionResult<WorkspaceInitializeCliCommand>;

public class WorkspaceInitializeCliCommandResult :
    CliCommandResult<WorkspaceInitializeCliCommand>,
    IWorkspaceInitializeCliCommandResult;