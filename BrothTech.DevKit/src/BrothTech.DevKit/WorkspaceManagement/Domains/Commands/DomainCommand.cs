using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.Commands;

public class DomainCommand :
    Command
{
    public DomainCommand() : 
        base(nameof(DomainCommand))
    {
        Aliases.Add("domain");
    }
}
