using BrothTech.Cli.Shared.CliCommands;
namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

public class ProjectAddCliCommand :
    CliCommand,
    IBaseProjectAddCliCommand,
    IHaveDomainNameOption
{
    public ProjectAddCliCommand() : 
        base(nameof(ProjectAddCliCommand))
    {
        ((IBaseProjectAddCliCommand)this).Add();
        ((IHaveDomainNameOption)this).Add();
    }

    protected override IEnumerable<string> GetAliases()
    {
        yield return "add";
    }
}
