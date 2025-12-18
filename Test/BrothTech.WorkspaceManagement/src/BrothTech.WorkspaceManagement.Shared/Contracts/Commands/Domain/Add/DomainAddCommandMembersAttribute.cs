using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Batch;
using BrothTech.WorkspaceManagement.Shared.Contracts.Workspaces;
using System.CommandLine;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Domain.Add;

public class DomainAddCommandMembersAttribute :
    CliCommandMembersAttribute
{
    public Option<bool> ShouldAddSharedProject { get; } = new(nameof(ShouldAddSharedProject), "--shared");

    public Option<bool> ShouldAddSandboxProject { get; } = new(nameof(ShouldAddSandboxProject), "--sandbox");

    public Option<bool> ShouldAddInternalProject { get; } = new(nameof(ShouldAddInternalProject), "--internal");

    protected override Dictionary<string, Symbol> Members => field ??= new()
    {
        [nameof(ShouldAddSharedProject)] = ShouldAddSharedProject,
        [nameof(ShouldAddSandboxProject)] = ShouldAddSandboxProject,
        [nameof(ShouldAddInternalProject)] = ShouldAddInternalProject
    };
}
