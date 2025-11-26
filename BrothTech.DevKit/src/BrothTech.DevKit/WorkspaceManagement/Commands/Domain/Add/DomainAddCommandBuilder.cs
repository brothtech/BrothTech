using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Domain.Add;

[ServiceDescriptor<ICommandBuilder<DomainCommand>, DomainAddCommandBuilder>]
public class DomainAddCommandBuilder(
    ILogger<DomainAddCommandBuilder> logger,
    IEnumerable<ICommandHandler<DomainAddCommand, DomainAddCommandResult>> handlers,
    IEnumerable<ICommandBuilder<DomainAddCommand>> builders) :
    BaseCommandBuilder<DomainCommand, DomainAddCommand, DomainAddCommandResult>(logger, handlers, builders),
    ICommandBuilder<DomainCommand>;