using BrothTech.Cli.Shared.CliCommands;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

public interface IHaveWorkspacePathOption :
    ICliCommand
{
    Option<string> WorkspacePath => GetOrCreateOption<string>($"--{nameof(WorkspacePath)}", "-w", "--workspace");

    void Add()
    {
        AddOption(WorkspacePath);
    }
}