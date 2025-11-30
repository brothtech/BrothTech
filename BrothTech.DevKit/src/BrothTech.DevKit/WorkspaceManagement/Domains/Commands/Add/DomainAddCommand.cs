using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.Commands.Add;

public class DomainAddCommand :
    BaseDomainAddCommand
{
    public Option<string> ParentDomainName { get; } = new($"--{nameof(ParentDomainName)}", "-p", "--parent");

    public DomainAddCommand() :
        base(nameof(DomainAddCommand))
    {
        Aliases.Add("add");
        Add(ParentDomainName);
    }
}
