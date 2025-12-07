using BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;
using System.CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.Commands.Add;

public abstract class BaseDomainAddCommandResult<TCommand> :
    BaseProjectAddCommandResult<TCommand>
    where TCommand : BaseDomainAddCommand, new()
{
    [NotNull]
    public bool? ShouldAddSharedProject 
    {
        get => field ??= ParseResult.GetValue(Command.ShouldAddSharedProject);
        set => field = value ?? throw new ArgumentNullException(nameof(value));
    }

    [NotNull]
    public bool? ShouldAddSandboxProject
    {
        get => field ??= ParseResult.GetValue(Command.ShouldAddSandboxProject);
        set => field = value ?? throw new ArgumentNullException(nameof(value));
    }

    [NotNull]
    public bool? ShouldAddInternalProject
    {
        get => field ??= ParseResult.GetValue(Command.ShouldAddInternalProject);
        set => field = value ?? throw new ArgumentNullException(nameof(value));
    }
}