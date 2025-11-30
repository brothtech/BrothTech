using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.Commands.Add;

[ServiceDescriptor<ICommandBuilder<DomainCommand>, DomainAddCommandBuilder>]
public class DomainAddCommandBuilder(
    ILogger<DomainAddCommandBuilder> logger,
    IEnumerable<ICommandBuilder<DomainAddCommand>> builders,
    IEnumerable<ICommandHandler<DomainAddCommand, DomainAddCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    BaseCommandBuilder<DomainCommand, DomainAddCommand, DomainAddCommandResult>(
        logger, 
        builders, 
        handlers, 
        commandInvoker);