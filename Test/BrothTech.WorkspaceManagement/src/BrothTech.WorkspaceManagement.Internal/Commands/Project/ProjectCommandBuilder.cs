using BrothTech.Cli.Shared;
using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Root;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.WorkspaceManagement.Shared.Contracts.Commands.Project;

[ServiceDescriptor<ICliCommandBuilder<RootCliCommand>, ProjectCommandBuilder>]
public class ProjectCommandBuilder(
    ILogger<ProjectCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<ProjectCommand>> childBuilders,
    ICliRequestInvoker requestInvoker) :
    CliCommandBuilder<RootCliCommand, ProjectCommand, ProjectRequest>(
        logger,
        childBuilders,
        requestInvoker);
