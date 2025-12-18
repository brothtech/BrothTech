using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Project.Add;
using System.CommandLine;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Domain.Add;

[NameArgument]
[ProjectAddCommandMembers]
[DomainAddCommandMembers]
[WorkspacePathOption]
public class DomainAddCommand :
    Command
{
    public DomainAddCommand() :
        base(nameof(DomainAddCommand))
    {
        Aliases.Add("add");
    }
}
