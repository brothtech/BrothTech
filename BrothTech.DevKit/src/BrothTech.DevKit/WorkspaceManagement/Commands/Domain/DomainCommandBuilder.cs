using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Commands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.DevKit.WorkspaceManagement.Commands.Domain;

[ServiceDescriptor<ICommandBuilder<RootCliCommand>, DomainCommandBuilder>]
public class DomainCommandBuilder(
    ILogger<DomainCommandBuilder> logger,
    IEnumerable<ICommandHandler<DomainCommand, DomainCommandResult>> handlers,
    IEnumerable<ICommandBuilder<DomainCommand>> builders) :
    BaseCommandBuilder<RootCliCommand, DomainCommand, DomainCommandResult>(logger, handlers, builders),
    ICommandBuilder<RootCliCommand>;
