using BrothTech.Cli.Shared.CliCommands;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

public interface IHaveWorkspacePathOptionResult<TCommand> :
    ICliCommandResult<TCommand>
    where TCommand : class, IHaveWorkspacePathOption, new()
{
    string? WorkspacePath
    {
        get => ParseResult.GetValue(Command.WorkspacePath);
        set => ParseResult.SetValue(Command.WorkspacePath, value.EnsureNotNull());
    }
}
