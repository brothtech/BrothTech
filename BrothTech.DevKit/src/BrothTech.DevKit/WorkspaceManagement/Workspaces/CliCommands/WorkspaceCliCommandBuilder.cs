using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.CliCommands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Workspaces.CliCommands;

[ServiceDescriptor<ICliCommandBuilder<RootCliCommand>, WorkspaceCliCommandBuilder>]
public class WorkspaceCliCommandBuilder(
    ILogger<WorkspaceCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<WorkspaceCliCommand>> builders,
    IEnumerable<ICliCommandValidator<WorkspaceCliCommand, WorkspaceCliCommandResult>> validators,
    IEnumerable<ICliCommandHandler<WorkspaceCliCommand, WorkspaceCliCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    CliCommandBuilder<RootCliCommand, WorkspaceCliCommand, WorkspaceCliCommandResult, WorkspaceCliCommandResult>(
        logger,
        builders,
        validators,
        handlers,
        commandInvoker);
