using BrothTech.Cli.Shared.CliCommands;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.WorkspaceManagement.Services;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

public interface IBaseProjectAddCliCommand :
    IHaveWorkspacePathOption
{
    Argument<string> Name => GetOrCreateArgument<string>(nameof(Name));

    Argument<ProjectExposureType> ExposureType => GetOrCreateArgument<ProjectExposureType>(nameof(ExposureType));

    Option<DotNetProjectTemplate> Template => GetOrCreateOption<DotNetProjectTemplate>($"--{nameof(Template)}", "-t", "--template");

    Option<string> FullyQualifiedName => GetOrCreateOption<string>($"--{nameof(FullyQualifiedName)}", "-qn", "--qualifiedname");

    new void Add()
    {
        ((IHaveWorkspacePathOption)this).Add();
        AddArgument(Name);
        AddArgument(ExposureType);
        AddOption(Template);
        AddOption(FullyQualifiedName);
    }
}
