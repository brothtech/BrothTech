using BrothTech.Cli.Shared.Contracts.Commands;
using System.CommandLine;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Project.Add;

[NameArgument]
[ProjectAddCommandMembers]
[DomainNameOption]
[WorkspacePathOption]
public class ProjectAddCommand :
    Command
{
    public ProjectAddCommand() :
        base(nameof(ProjectAddCommand))
    {
        Aliases.Add("add");
    }
}
