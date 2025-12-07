using BrothTech.Cli.Shared.CliCommands;
using BrothTech.DevKit.Infrastructure.DotNet;
using BrothTech.DevKit.WorkspaceManagement.Services;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

public interface IBaseProjectAddCliCommand :
    ICliCommand
{
    Argument<string> Name => GetOrCreateArgument<string>(nameof(Name));

    Argument<ProjectExposureType> ExposureType => GetOrCreateArgument<ProjectExposureType>(nameof(ExposureType));

    Option<string> WorkspacePath => GetOrCreateOption<string>($"--{nameof(WorkspacePath)}", "-w", "--workspace");

    Option<DotNetProjectTemplate> Template => GetOrCreateOption<DotNetProjectTemplate>($"--{nameof(Template)}", "-t", "--template");

    Option<string> FullyQualifiedName => GetOrCreateOption<string>($"--{nameof(FullyQualifiedName)}", "-qn", "--qualifiedname");

    void Add()
    {
        AddArgument(Name);
        AddArgument(ExposureType);
        AddOption(WorkspacePath);
        AddOption(Template);
        AddOption(FullyQualifiedName);
    }
}
