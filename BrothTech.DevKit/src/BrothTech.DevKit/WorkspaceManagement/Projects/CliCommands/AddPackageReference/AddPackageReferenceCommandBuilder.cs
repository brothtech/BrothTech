using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Projects.CliCommands.AddPackageReference;

[ServiceDescriptor<ICliCommandBuilder<ProjectCliCommand>, AddPackageReferenceCommandBuilder>]
public class AddPackageReferenceCommandBuilder(
    ILogger<AddPackageReferenceCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<AddPackageReferenceCommand>> builders,
    IEnumerable<ICliCommandHandler<AddPackageReferenceCommand, IAddPackageReferenceCommandResult>> handlers,
    ICliCommandInvoker CliCommandInvoker) :
    CliCommandBuilder<ProjectCliCommand, AddPackageReferenceCommand, AddPackageReferenceCommandResult, IAddPackageReferenceCommandResult>(
        logger, 
        builders, 
        handlers, 
        CliCommandInvoker);