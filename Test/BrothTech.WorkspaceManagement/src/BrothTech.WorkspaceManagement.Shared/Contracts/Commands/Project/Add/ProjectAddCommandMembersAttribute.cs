using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Batch;
using BrothTech.WorkspaceManagement.Shared.Contracts.Workspaces;
using System.CommandLine;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Project.Add;

public class ProjectAddCommandMembersAttribute :
    CliCommandMembersAttribute
{
    public Argument<ProjectExposureType> ExposureType { get; } = new(nameof(ExposureType));

    public Option<string> Template { get; } = new(nameof(Template));

    public Option<string> FullyQualifiedName { get; } = new(nameof(FullyQualifiedName));

    protected override Dictionary<string, Symbol> Members => field ??= new()
    {
        [nameof(ExposureType)] = ExposureType,
        [nameof(Template)] = Template,
        [nameof(FullyQualifiedName)] = FullyQualifiedName
    };
}
