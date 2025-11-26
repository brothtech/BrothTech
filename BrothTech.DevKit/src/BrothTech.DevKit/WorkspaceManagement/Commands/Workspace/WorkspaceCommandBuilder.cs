using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Commands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Workspace;

[ServiceDescriptor<ICommandBuilder<RootCliCommand>, WorkspaceCommandBuilder>]
public class WorkspaceCommandBuilder(
    ILogger<WorkspaceCommandBuilder> logger,
    IEnumerable<ICommandHandler<WorkspaceCommand, WorkspaceCommandResult>> handlers,
    IEnumerable<ICommandBuilder<WorkspaceCommand>> builders) :
    BaseCommandBuilder<RootCliCommand, WorkspaceCommand, WorkspaceCommandResult>(logger, handlers, builders),
    ICommandBuilder<RootCliCommand>;
