using BrothTech.Cli.Shared.CliCommands;
using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.AddPackageReference;

public class AddPackageReferenceCommand :
    CliCommand,
    IHaveWorkspacePathOption
{
    public Argument<string> DomainName => GetOrCreateArgument<string>(nameof(DomainName));

    public Argument<string> ProjectName => GetOrCreateArgument<string>(nameof(ProjectName));

    public Argument<string> PackageName => GetOrCreateArgument<string>(nameof(PackageName));

    public Argument<string> PackageVersion => GetOrCreateArgument<string>(nameof(PackageVersion));

    public AddPackageReferenceCommand() :
        base(nameof(AddPackageReferenceCommand))
    {
        ((IHaveWorkspacePathOption)this).Add();
        AddArgument(DomainName);
        AddArgument(ProjectName);
        AddArgument(PackageName);
        AddArgument(PackageVersion);
    }

    protected override IEnumerable<string> GetAliases()
    {
        yield return "add-package";
    }
}
