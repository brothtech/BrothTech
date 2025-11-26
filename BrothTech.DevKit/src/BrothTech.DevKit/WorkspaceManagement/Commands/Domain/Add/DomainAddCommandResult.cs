using BrothTech.Cli.Shared.Commands;
using BrothTech.DevKit.Infrastructure.DotNet;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Domain.Add;

public class DomainAddCommandResult :
    BaseCommandResult<DomainAddCommand>
{
    public string Name => field ??= ParseResult.GetRequiredValue(Command.Name);

    public DotNetProjectTemplate? PrimaryProjectType => field ??= ParseResult.GetValue(Command.PrimaryProjectType);
}
