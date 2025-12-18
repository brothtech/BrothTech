using BrothTech.Cli.Shared.Contracts.Commands;
using System.CommandLine;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands;

public class WorkspacePathOptionAttribute :
    CliCommandMembersAttribute
{
    public Option<string> WorkspacePath { get; } = new($"--{nameof(WorkspacePath)}", "-w", "--workspace");

    protected override Dictionary<string, Symbol> Members => field ??= new()
    {
        [nameof(WorkspacePath)] = WorkspacePath
    };
}