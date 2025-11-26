using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Workspace.Initialize;

[ServiceDescriptor<ICommandBuilder<WorkspaceCommand>, WorkspaceInitializeCommandBuilder>]
public class WorkspaceInitializeCommandBuilder(
    ILogger<WorkspaceInitializeCommandBuilder> logger,
    IEnumerable<ICommandHandler<WorkspaceInitializeCommand, WorkspaceInitializeCommandResult>> handlers,
    IEnumerable<ICommandBuilder<WorkspaceInitializeCommand>> builders) :
    BaseCommandBuilder<WorkspaceCommand, WorkspaceInitializeCommand, WorkspaceInitializeCommandResult>(logger, handlers, builders),
    ICommandBuilder<WorkspaceCommand>;