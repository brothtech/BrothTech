using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.CliCommands.Batch;
using BrothTech.Cli.Shared.CliCommands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.Cli.CliCommands.Batch;

[ServiceDescriptor<ICliCommandBuilder<RootCliCommand>, BatchCliCommandBuilder>]
public class BatchCliCommandBuilder(
    ILogger<BatchCliCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<BatchCliCommand>> builders,
    IEnumerable<ICliCommandHandler<BatchCliCommand, BatchCliCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    CliCommandBuilder<RootCliCommand, BatchCliCommand, BatchCliCommandResult, BatchCliCommandResult>(
        logger, 
        builders, 
        handlers, 
        commandInvoker);