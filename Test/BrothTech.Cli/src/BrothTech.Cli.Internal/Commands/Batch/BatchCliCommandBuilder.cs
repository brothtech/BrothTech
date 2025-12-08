using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Batch;
using BrothTech.Cli.Shared.Contracts.Commands.Root;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.Cli.Internal.Commands.Batch;

[ServiceDescriptor<ICliCommandBuilder<RootCliCommand>, BatchCliCommandBuilder>]
public class BatchCliCommandBuilder(
    ILogger<BatchCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<BatchCommand>> builders,
    IEnumerable<ICliCommandHandler<BatchCommand, BatchCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    CliCommandBuilder<RootCliCommand, BatchCommand, BatchCommandResult, BatchCommandResult>(
        logger, 
        builders, 
        handlers, 
        commandInvoker);