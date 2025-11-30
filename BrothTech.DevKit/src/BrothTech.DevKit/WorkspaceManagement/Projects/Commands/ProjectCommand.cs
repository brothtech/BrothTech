using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.Commands;

public class ProjectCommand :
    Command
{
    public ProjectCommand() : 
        base(nameof(ProjectCommand))
    {
        Aliases.Add("project");
    }
}
