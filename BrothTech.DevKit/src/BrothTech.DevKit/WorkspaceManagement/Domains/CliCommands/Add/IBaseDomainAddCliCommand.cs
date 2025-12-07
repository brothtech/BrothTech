using BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;
using System.CommandLine;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands.Add;

public interface IBaseDomainAddCliCommand :
    IBaseProjectAddCliCommand
{
    Option<bool> ShouldAddSharedProject => GetOrCreateOption<bool>($"--{nameof(ShouldAddSharedProject)}", "--addshared");

    Option<bool> ShouldAddSandboxProject => GetOrCreateOption<bool>($"--{nameof(ShouldAddSandboxProject)}", "--addsandbox");

    Option<bool> ShouldAddInternalProject => GetOrCreateOption<bool>($"--{nameof(ShouldAddInternalProject)}", "--addinternal");

    new void Add()
    {
        ((IBaseProjectAddCliCommand)this).Add();
        AddOption(ShouldAddSharedProject);
        AddOption(ShouldAddSandboxProject);
        AddOption(ShouldAddInternalProject);
    }
}
