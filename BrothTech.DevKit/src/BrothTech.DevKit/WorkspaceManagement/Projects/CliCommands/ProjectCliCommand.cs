using BrothTech.Cli.Shared.CliCommands;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands;

public class ProjectCliCommand() :
    CliCommand(nameof(ProjectCliCommand))
{
    protected override IEnumerable<string> GetAliases()
    {
        yield return "project";
    }
}
