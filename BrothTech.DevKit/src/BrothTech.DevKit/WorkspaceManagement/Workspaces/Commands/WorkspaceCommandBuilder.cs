using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Commands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.Commands;

[ServiceDescriptor<ICommandBuilder<RootCliCommand>, WorkspaceCommandBuilder>]
public class WorkspaceCommandBuilder(
    ILogger<WorkspaceCommandBuilder> logger,
    IEnumerable<ICommandBuilder<WorkspaceCommand>> builders,
    IEnumerable<ICommandHandler<WorkspaceCommand, WorkspaceCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    BaseCommandBuilder<RootCliCommand, WorkspaceCommand, WorkspaceCommandResult>(
        logger, 
        builders, 
        handlers, 
        commandInvoker);
