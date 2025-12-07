using BrothTech.Cli.Shared.CliCommands;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.WorkspaceManagement.Services;
using System.Diagnostics.CodeAnalysis;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

public interface IBaseProjectAddCliCommandResult<TCommand> :
    ICliCommandResult<TCommand>
    where TCommand : class, IBaseProjectAddCliCommand, new()
{
    string Name
    {
        get => ParseResult.GetRequiredValue(Command.Name);
        set => ParseResult.SetValue(Command.Name, value.EnsureNotNull());
    }

    [NotNull]
    ProjectExposureType? ExposureType
    {
        get => ParseResult.GetRequiredValue(Command.ExposureType);
        set => ParseResult.SetValue(Command.ExposureType, value ?? throw new ArgumentNullException(nameof(value)));
    }

    string? WorkspacePath
    {
        get => ParseResult.GetValue(Command.WorkspacePath);
        set => ParseResult.SetValue(Command.WorkspacePath, value.EnsureNotNull());
    }

    DotNetProjectTemplate? Template
    {
        get => ParseResult.GetValue(Command.Template);
        set => ParseResult.SetValue(Command.Template, value ?? throw new ArgumentNullException(nameof(value)));
    }

    string? FullyQualifiedName
    {
        get => ParseResult.GetValue(Command.FullyQualifiedName);
        set => ParseResult.SetValue(Command.FullyQualifiedName, value.EnsureNotNull());
    }
}
