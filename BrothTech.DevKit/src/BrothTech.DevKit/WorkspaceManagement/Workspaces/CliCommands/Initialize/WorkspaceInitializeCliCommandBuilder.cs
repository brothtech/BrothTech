using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.CliCommands.Initialize;

[ServiceDescriptor<ICliCommandBuilder<WorkspaceCliCommand>, WorkspaceInitializeCliCommandBuilder>]
public class WorkspaceInitializeCliCommandBuilder(
    ILogger<WorkspaceInitializeCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<WorkspaceInitializeCliCommand>> builders,
    IEnumerable<ICliCommandValidator<WorkspaceInitializeCliCommand, IWorkspaceInitializeCliCommandResult>> validators,
    IEnumerable<ICliCommandHandler<WorkspaceInitializeCliCommand, IWorkspaceInitializeCliCommandResult>> handlers,
    ICliCommandInvoker CliCommandInvoker) :
    CliCommandBuilder<WorkspaceCliCommand, WorkspaceInitializeCliCommand, WorkspaceInitializeCliCommandResult, IWorkspaceInitializeCliCommandResult>(
        logger, 
        builders, 
        validators,
        handlers, 
        CliCommandInvoker);