using BrothTech.Cli.Shared;
using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Batch;
using BrothTech.Cli.Shared.Contracts.Commands.Root;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrothTech.Cli.Internal.Commands.Batch;

[ServiceDescriptor<ICliCommandBuilder<RootCliCommand>, BatchCommandBuilder>]
internal class BatchCommandBuilder(
    ILogger<BatchCommandBuilder> logger,
    IEnumerable<ICliCommandBuilder<BatchCommand>> childBuilders,
    ICliRequestInvoker requestInvoker) :
    CliCommandBuilder<RootCliCommand, BatchCommand, BatchRequest>(
        logger, 
        childBuilders, 
        requestInvoker);