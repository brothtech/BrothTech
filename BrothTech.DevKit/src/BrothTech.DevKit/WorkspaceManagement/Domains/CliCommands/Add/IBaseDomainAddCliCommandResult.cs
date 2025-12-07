using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;
using System.Diagnostics.CodeAnalysis;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands.Add;

public interface IBaseDomainAddCliCommandResult<TCommand> :
    IBaseProjectAddCliCommandResult<TCommand>
    where TCommand : class, IBaseDomainAddCliCommand, new()
{
    [NotNull]
    bool? ShouldAddSharedProject
    {
        get => ParseResult.GetRequiredValue(Command.ShouldAddSharedProject);
        set => ParseResult.SetValue(Command.ShouldAddSharedProject, value ?? throw new ArgumentNullException(nameof(value)));
    }

    [NotNull]
    bool? ShouldAddSandboxProject
    {
        get => ParseResult.GetRequiredValue(Command.ShouldAddSandboxProject);
        set => ParseResult.SetValue(Command.ShouldAddSandboxProject, value ?? throw new ArgumentNullException(nameof(value)));
    }

    [NotNull]
    bool? ShouldAddInternalProject
    {
        get => ParseResult.GetRequiredValue(Command.ShouldAddInternalProject);
        set => ParseResult.SetValue(Command.ShouldAddInternalProject, value ?? throw new ArgumentNullException(nameof(value)));
    }
}