using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.WorkspaceManagement.Services;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;

public abstract class BaseProjectAddCommand :
    Command
{
    public new Argument<string> Name { get; } = new(nameof(Name));

    public Argument<ProjectExposureType> ExposureType { get; } = new(nameof(ExposureType));

    public Option<string> WorkspacePath { get; } = new($"--{nameof(WorkspacePath)}", "-w", "--workspace");

    public Option<string> DomainName { get; } = new($"--{nameof(DomainName)}", "-d", "--domain");

    public Option<DotNetProjectTemplate> Template { get; } = new($"--{nameof(Template)}", "-t", "--template");

    public BaseProjectAddCommand(
        string name,
        string? description = null) :
        base(name, description)
    {
        Add(Name);
        Add(ExposureType);
        Add(WorkspacePath);
        Add(DomainName);
        Add(Template);
    }
}