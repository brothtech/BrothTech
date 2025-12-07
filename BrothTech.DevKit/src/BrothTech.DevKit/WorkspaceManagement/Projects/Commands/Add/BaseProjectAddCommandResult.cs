using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.WorkspaceManagement.Services;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;

public abstract class BaseProjectAddCommandResult<TCommand>() :
    BaseCommandResult<TCommand>
    where TCommand : BaseProjectAddCommand, new()
{
    public string Name
    {
        get => field ??= ParseResult.GetRequiredValue(Command.Name);
        set => field = value.EnsureNotNull();
    }

    [NotNull]
    public ProjectExposureType? ExposureType
    {
        get => field ??= ParseResult.GetRequiredValue(Command.ExposureType);
        set => field = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string? WorkspacePath
    {
        get => field ??= ParseResult.GetValue(Command.WorkspacePath);
        set => field = value;
    }

    public string? DomainName
    {
        get => field ??= ParseResult.GetValue(Command.DomainName);
        set => field = value;
    }

    public DotNetProjectTemplate? Template
    {
        get => field ??= ParseResult.GetValue(Command.Template);
        set => field = value;
    }

    public string? FullyQualifiedName
    {
        get => field ??= ParseResult.GetValue(Command.FullyQualifiedName);
        set => field = value;
    }
}
