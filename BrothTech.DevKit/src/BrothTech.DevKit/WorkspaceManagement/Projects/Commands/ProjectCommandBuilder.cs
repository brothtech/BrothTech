using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Commands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.Commands;

[ServiceDescriptor<ICommandBuilder<RootCliCommand>, ProjectCommandBuilder>]
public class ProjectCommandBuilder(
    ILogger<ProjectCommandBuilder> logger,
    IEnumerable<ICommandBuilder<ProjectCommand>> builders,
    IEnumerable<ICommandHandler<ProjectCommand, ProjectCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    BaseCommandBuilder<RootCliCommand, ProjectCommand, ProjectCommandResult>(
        logger, 
        builders, 
        handlers, 
        commandInvoker);
