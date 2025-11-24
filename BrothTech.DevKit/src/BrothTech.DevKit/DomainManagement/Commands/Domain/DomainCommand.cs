using System.CommandLine;

namespace BrothTech.DevKit.DomainManagement.Commands.Domain;

public class DomainCommand :
    Command
{
    public DomainCommand() : 
        base(nameof(DomainCommand))
    {
        Aliases.Add("domain");
    }
}