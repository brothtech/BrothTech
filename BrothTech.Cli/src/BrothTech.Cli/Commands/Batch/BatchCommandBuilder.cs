using BrothTech.Cli.Shared.Commands;
using BrothTech.Cli.Shared.Commands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.Cli.Commands.Batch;

[ServiceDescriptor<ICommandBuilder<RootCliCommand>, BatchCommandBuilder>]
public class BatchCommandBuilder(
    ILogger<BatchCommandBuilder> logger,
    IEnumerable<ICommandBuilder<BatchCommand>> builders,
    IEnumerable<ICommandHandler<BatchCommand, BatchCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    BaseCommandBuilder<RootCliCommand, BatchCommand, BatchCommandResult>(
        logger, 
        builders, 
        handlers, 
        commandInvoker);