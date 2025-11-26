using BrothTech.DevKit.Infrastructure.DotNet;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Domain.Add;

public class DomainAddCommand :
    Command
{
    public DomainAddCommand() :
        base(nameof(DomainAddCommand))
    {
        Aliases.Add("add");
        Add(Name);
        Add(PrimaryProjectType);
    }

    public new Argument<string> Name { get; } = new(nameof(Name));

    public Option<DotNetProjectTemplate> PrimaryProjectType { get; } = new(nameof(PrimaryProjectType), "-t", "--type");
}
