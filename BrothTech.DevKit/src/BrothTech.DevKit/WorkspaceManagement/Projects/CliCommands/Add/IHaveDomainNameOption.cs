using BrothTech.Cli.Shared.CliCommands;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

public interface IHaveDomainNameOption :
    ICliCommand
{
    Option<string> DomainName => GetOrCreateOption<string>($"--{nameof(DomainName)}", "-d", "--domain");

    void Add()
    {
        AddOption(DomainName);
    }
}