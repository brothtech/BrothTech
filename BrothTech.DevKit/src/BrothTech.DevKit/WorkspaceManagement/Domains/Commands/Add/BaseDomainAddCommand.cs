using BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.Commands.Add;

public abstract class BaseDomainAddCommand :
    BaseProjectAddCommand
{
    public Option<bool> ShouldAddSharedProject { get; } = new($"--{nameof(ShouldAddSharedProject)}", "--addshared");

    public Option<bool> ShouldAddSandboxProject { get; } = new($"--{nameof(ShouldAddSandboxProject)}", "--addsandbox");

    public Option<bool> ShouldAddInternalProject { get; } = new($"--{nameof(ShouldAddInternalProject)}", "--addinternal");

    public BaseDomainAddCommand(
        string name,
        string? description = null) :
        base(name, description)
    {
        Add(ShouldAddSharedProject);
        Add(ShouldAddSandboxProject);
        Add(ShouldAddInternalProject);
    }
}