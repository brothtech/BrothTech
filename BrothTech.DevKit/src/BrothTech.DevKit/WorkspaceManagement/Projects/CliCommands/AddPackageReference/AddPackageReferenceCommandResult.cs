using BrothTech.Cli.Shared.CliCommands;
using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.AddPackageReference;

public interface IAddPackageReferenceCommandResult :
    IHaveWorkspacePathOptionResult<AddPackageReferenceCommand>
{
    public string DomainName
    {
        get => ParseResult.GetRequiredValue(Command.DomainName);
        set => ParseResult.SetValue(Command.DomainName, value.EnsureNotNull());
    }

    public string ProjectName
    {
        get => ParseResult.GetRequiredValue(Command.ProjectName);
        set => ParseResult.SetValue(Command.ProjectName, value.EnsureNotNull());
    }

    public string PackageName
    {
        get => ParseResult.GetRequiredValue(Command.PackageName);
        set => ParseResult.SetValue(Command.PackageName, value.EnsureNotNull());
    }

    public string PackageVersion
    {
        get => ParseResult.GetRequiredValue(Command.PackageVersion);
        set => ParseResult.SetValue(Command.PackageVersion, value.EnsureNotNull());
    }
}

public class AddPackageReferenceCommandResult :
    CliCommandResult<AddPackageReferenceCommand>,
    IAddPackageReferenceCommandResult;