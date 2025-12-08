using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Root;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Workspace;

[ServiceDescriptor<ICliCommandBuilder<RootCliCommand>, WorkspaceCliCommandBuilder>]
public class WorkspaceCliCommandBuilder(
    ILogger<WorkspaceCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<WorkspaceCliCommand>> builders,
    IEnumerable<ICliCommandHandler<WorkspaceCliCommand, WorkspaceCliCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    CliCommandBuilder<RootCliCommand, WorkspaceCliCommand, WorkspaceCliCommandResult, WorkspaceCliCommandResult>(
        logger,
        builders,
        handlers,
        commandInvoker);
