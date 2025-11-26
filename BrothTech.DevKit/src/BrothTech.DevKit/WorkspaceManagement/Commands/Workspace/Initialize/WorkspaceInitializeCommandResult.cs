using BrothTech.Cli.Shared.Commands;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.WorkspaceManagement.Services;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Workspace.Initialize;

public class WorkspaceInitializeCommandResult :
    BaseCommandResult<WorkspaceInitializeCommand>
{
    public string Name => field ??= ParseResult.GetRequiredValue(Command.Name);

    public DotNetProjectTemplate? Template => field ??= ParseResult.GetValue(Command.Template);

    public ProjectExposureType? Exposure => field ??= ParseResult.GetValue(Command.Exposure);
}
