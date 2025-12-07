using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.Add;

[ServiceDescriptor<ICliCommandBuilder<ProjectCliCommand>, ProjectAddCliCommandBuilder>]
public class ProjectAddCliCommandBuilder(
    ILogger<ProjectAddCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<ProjectAddCliCommand>> builders,
    IEnumerable<ICliCommandHandler<ProjectAddCliCommand, IProjectAddCliCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    CliCommandBuilder<ProjectCliCommand, ProjectAddCliCommand, ProjectAddCliCommandResult, IProjectAddCliCommandResult>(
        logger, 
        builders, 
        handlers, 
        commandInvoker);