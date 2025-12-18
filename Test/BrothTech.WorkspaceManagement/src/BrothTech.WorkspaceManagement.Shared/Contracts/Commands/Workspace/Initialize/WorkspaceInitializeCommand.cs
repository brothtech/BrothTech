using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Domain.Add;
using BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Project.Add;
using System.CommandLine;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Workspace.Initialize;

[NameArgument]
[ProjectAddCommandMembers]
[DomainAddCommandMembers]
[WorkspacePathOption]
public class WorkspaceInitializeCommand :
    Command
{
    public WorkspaceInitializeCommand() :
        base(nameof(WorkspaceInitializeCommand))
    {
        Aliases.Add("initialize");
        Aliases.Add("init");
    }
}
