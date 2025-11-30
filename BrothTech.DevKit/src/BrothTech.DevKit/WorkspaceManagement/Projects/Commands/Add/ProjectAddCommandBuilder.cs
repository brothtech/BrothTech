using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.Commands.Add;

[ServiceDescriptor<ICommandBuilder<ProjectCommand>, ProjectAddCommandBuilder>]
public class ProjectAddCommandBuilder(
    ILogger<ProjectAddCommandBuilder> logger,
    IEnumerable<ICommandBuilder<ProjectAddCommand>> builders,
    IEnumerable<ICommandHandler<ProjectAddCommand, ProjectAddCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    BaseCommandBuilder<ProjectCommand, ProjectAddCommand, ProjectAddCommandResult>(
        logger, 
        builders, 
        handlers, 
        commandInvoker);