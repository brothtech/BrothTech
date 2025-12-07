using BrothTech.Cli.Shared.CliCommands;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

public interface IProjectAddCliCommandResult :
    IBaseProjectAddCliCommandResult<ProjectAddCliCommand>,
    IHaveDomainNameOptionResult<ProjectAddCliCommand>;

public class ProjectAddCliCommandResult :
    CliCommandResult<ProjectAddCliCommand>,
    IProjectAddCliCommandResult;