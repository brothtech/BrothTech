using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands.Add;

[ServiceDescriptor<ICliCommandBuilder<DomainCliCommand>, DomainAddCliCommandBuilder>]
public class DomainAddCliCommandBuilder(
    ILogger<DomainAddCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<DomainAddCliCommand>> builders,
    IEnumerable<ICliCommandHandler<DomainAddCliCommand, IDomainAddCliCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    CliCommandBuilder<DomainCliCommand, DomainAddCliCommand, DomainAddCliCommandResult, IDomainAddCliCommandResult>(
        logger, 
        builders, 
        handlers, 
        commandInvoker);