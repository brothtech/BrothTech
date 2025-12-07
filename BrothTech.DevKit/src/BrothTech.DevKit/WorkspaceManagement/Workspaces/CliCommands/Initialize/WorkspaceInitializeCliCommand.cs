using BrothTech.Cli.Shared.CliCommands;
using BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands.Add;
using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.CliCommands.Initialize;

public class WorkspaceInitializeCliCommand :
    CliCommand,
    IBaseDomainAddCliCommand,
    IHaveDomainNameOption
{
    public WorkspaceInitializeCliCommand() : 
        base(nameof(WorkspaceInitializeCliCommand))
    {
        ((IBaseDomainAddCliCommand)this).Add();
    }

    protected override IEnumerable<string> GetAliases()
    {
        yield return "initialize";
        yield return "init";
    }
}
