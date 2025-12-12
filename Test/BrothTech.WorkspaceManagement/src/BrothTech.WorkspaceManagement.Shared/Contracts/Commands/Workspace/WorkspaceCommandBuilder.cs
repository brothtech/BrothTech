using BrothTech.Cli.Shared;
using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Root;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Workspace;

[ServiceDescriptor<ICliCommandBuilder<RootCliCommand>, WorkspaceCommandBuilder>]
public class WorkspaceCommandBuilder(
    ILogger<WorkspaceCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<WorkspaceCommand>> childBuilders,
    ICliRequestInvoker requestInvoker) :
    CliCommandBuilder<RootCliCommand, WorkspaceCommand, WorkspaceRequest>(
        logger,
        childBuilders,
        requestInvoker);
