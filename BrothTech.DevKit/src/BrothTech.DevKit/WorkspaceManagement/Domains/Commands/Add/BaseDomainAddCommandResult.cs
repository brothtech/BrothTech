using BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.Commands.Add;

public abstract class BaseDomainAddCommandResult<TCommand> :
    BaseProjectAddCommandResult<TCommand>
    where TCommand : BaseDomainAddCommand, new()
{
    [NotNull]
    public bool? NoSharedProject 
    {
        get => field ??= ParseResult.GetValue(Command.NoSharedProject);
        set => field = value ?? throw new ArgumentNullException(nameof(value));
    }

    [NotNull]
    public bool? NoSandboxProject
    {
        get => field ??= ParseResult.GetValue(Command.NoSandboxProject);
        set => field = value ?? throw new ArgumentNullException(nameof(value));
    }
}