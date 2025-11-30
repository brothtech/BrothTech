using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.Commands.Initialize;

[ServiceDescriptor<ICommandBuilder<WorkspaceCommand>, WorkspaceInitializeCommandBuilder>]
public class WorkspaceInitializeCommandBuilder(
    ILogger<WorkspaceInitializeCommandBuilder> logger,
    IEnumerable<ICommandBuilder<WorkspaceInitializeCommand>> builders,
    IEnumerable<ICommandHandler<WorkspaceInitializeCommand, WorkspaceInitializeCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    BaseCommandBuilder<WorkspaceCommand, WorkspaceInitializeCommand, WorkspaceInitializeCommandResult>(
        logger, 
        builders, 
        handlers, 
        commandInvoker);