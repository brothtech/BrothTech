using BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.Commands.Add;

public abstract class BaseDomainAddCommand :
    BaseProjectAddCommand
{
    public Option<bool> NoSharedProject { get; } = new($"--{nameof(NoSharedProject)}", "--noshared");

    public Option<bool> NoSandboxProject { get; } = new($"--{nameof(NoSandboxProject)}", "--nosandbox");

    public BaseDomainAddCommand(
        string name,
        string? description = null) :
        base(name, description)
    {
        Add(NoSharedProject);
        Add(NoSandboxProject);
    }
}