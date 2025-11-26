using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.WorkspaceManagement.Services;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Workspace.Initialize;

public class WorkspaceInitializeCommand :
    Command
{
    public WorkspaceInitializeCommand() :
        base(nameof(WorkspaceInitializeCommand))
    {
        Aliases.Add("initialize");
        Aliases.Add("init");
        Add(Name);
        Add(Template);
        Add(Exposure);
    }

    public new Argument<string> Name { get; } = new(nameof(Name));

    public Option<DotNetProjectTemplate> Template { get; } = new(nameof(Template), "-t", "--template");

    public Option<ProjectExposureType> Exposure { get; } = new(nameof(Exposure), "-e", "--exposure");
}
