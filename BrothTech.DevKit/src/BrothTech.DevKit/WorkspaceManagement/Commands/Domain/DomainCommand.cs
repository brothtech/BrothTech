using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Domain;

public class DomainCommand :
    Command
{
    public DomainCommand() : 
        base(nameof(DomainCommand))
    {
        Aliases.Add("domain");
    }
}
