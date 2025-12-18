using System.CommandLine;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Project;

public class ProjectCommand :
    Command
{
    public ProjectCommand() : 
        base(nameof(ProjectCommand))
    {
        Aliases.Add("project");
    }
}
