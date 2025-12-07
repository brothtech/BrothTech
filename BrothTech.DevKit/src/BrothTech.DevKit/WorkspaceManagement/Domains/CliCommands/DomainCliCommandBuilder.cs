using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.CliCommands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands;

[ServiceDescriptor<ICliCommandBuilder<RootCliCommand>, DomainCliCommandBuilder>]
public class DomainCliCommandBuilder(
    ILogger<DomainCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<DomainCliCommand>> builders,
    IEnumerable<ICliCommandHandler<DomainCliCommand, DomainCliCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    CliCommandBuilder<RootCliCommand, DomainCliCommand, DomainCliCommandResult, DomainCliCommandResult>(
        logger,
        builders,
        handlers,
        commandInvoker);
