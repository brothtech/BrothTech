using BrothTech.Cli.Shared.CliCommands;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

public interface IHaveDomainNameOptionResult<TCommand> :
    ICliCommandResult<TCommand>
    where TCommand : class, IHaveDomainNameOption, new()
{
    string? DomainName
    {
        get => ParseResult.GetValue(Command.DomainName);
        set => ParseResult.SetValue(Command.DomainName, value.EnsureNotNull());
    }
}
