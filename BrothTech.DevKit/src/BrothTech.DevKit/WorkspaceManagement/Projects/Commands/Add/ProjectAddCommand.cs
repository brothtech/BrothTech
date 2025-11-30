namespace BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;

public class ProjectAddCommand :
    BaseProjectAddCommand
{
    public ProjectAddCommand() :
        base(nameof(ProjectAddCommand))
    {
        Aliases.Add("add");
    }
}
