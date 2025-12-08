using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.CliCommands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands;

[ServiceDescriptor<ICliCommandBuilder<RootCliCommand>, ProjectCliCommandBuilder>]
public class ProjectCliCommandBuilder(
    ILogger<ProjectCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<ProjectCliCommand>> builders,
    IEnumerable<ICliCommandValidator<ProjectCliCommand, ProjectCliCommandResult>> validators,
    IEnumerable<ICliCommandHandler<ProjectCliCommand, ProjectCliCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    CliCommandBuilder<RootCliCommand, ProjectCliCommand, ProjectCliCommandResult, ProjectCliCommandResult>(
        logger,
        builders,
        validators,
        handlers,
        commandInvoker);
