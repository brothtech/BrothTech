using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace BrothTech.Cli.Shared.CliCommands;

public interface ICliCommandBuilder<TParentCommand> :
    ICliCommandBuilder
    where TParentCommand : class, ICliCommand, new();

public interface ICliCommandBuilder
{
    Result<ICliCommand> TryBuild();
}

public abstract class CliCommandBuilder<TParentCommand, TCommand, TCommandResultConcrete, TCommandResultContract>(
    ILogger logger,
    IEnumerable<ICliCommandBuilder<TCommand>> childBuilders,
    IEnumerable<ICliCommandHandler<TCommand, TCommandResultContract>> handlers,
    ICliCommandInvoker commandInvoker) :
    ICliCommandBuilder<TParentCommand>
    where TParentCommand : class, ICliCommand, new()
    where TCommand : class, ICliCommand, new()
    where TCommandResultContract : ICliCommandResult<TCommand>
    where TCommandResultConcrete : class, TCommandResultContract, new()
{
    private readonly ILogger _logger = logger.EnsureNotNull();
    private readonly IEnumerable<ICliCommandBuilder<TCommand>> _childBuilders = childBuilders.EnsureNotNull();
    private readonly IEnumerable<ICliCommandHandler<TCommand, TCommandResultContract>> _handlers = handlers.EnsureNotNull();
    private readonly ICliCommandInvoker _commandInvoker = commandInvoker.EnsureNotNull();
    private TCommand? _command;

    public virtual Result<ICliCommand> TryBuild()
    {
        if (_command is not null)
            return _command;

        _command = new TCommand();
        if (TryAddChildren(_command).HasFailed(out _, out var messages))
            return ErrorResult.FromMessages(messages);

        _command.SetHandler(HandleAsync);
        OnBuilt(_command);
        return _command;
    }

    private Result TryAddChildren(
        TCommand command)
    {
        var aggregateResult = Result.Success;
        foreach (var builder in _childBuilders)
        {
            aggregateResult &= builder.TryBuild().OutWithItem(out var child);
            if (aggregateResult.IsSuccessful is false)
                return aggregateResult;

            command.AddCommand(child);
        }

        return aggregateResult;
    }

    private async Task HandleAsync(
        ParseResult parseResult,
        CancellationToken token)
    {
        var command = _command.EnsureNotNull();
        var commandResult = new TCommandResultConcrete()
        {
            Command = command,
            ParseResult = parseResult
        };
        var result = await TryHandleAsync(commandResult, token);
        if (result.IsSuccessful)
            return;

        foreach (var message in result.Messages)
            _logger.Log(message.LogLevel, message.Message, message.Args);
    }

    public async Task<Result> TryHandleAsync(
        TCommandResultContract commandResult,
        CancellationToken token)
    {
        var aggregateResult = Result.Success;
        foreach (var handler in _handlers.OrderBy(x => x.Priority))
        {
            aggregateResult &= await TryExecuteHandlerAsync(commandResult, handler, token);
            if (aggregateResult.IsSuccessful is false)
                return aggregateResult;
        }

        return aggregateResult;
    }

    private async Task<Result> TryExecuteHandlerAsync(
        TCommandResultContract commandResult,
        ICliCommandHandler<TCommand, TCommandResultContract> handler,
        CancellationToken token)
    {
        try
        {
            var handlerResult = await handler.TryHandleAsync(commandResult, token);
            if (handlerResult.IsSuccessful && handler.ShouldInvokeNewCommands(commandResult))
                await InvokeNewCommandAsync(handler, commandResult, token);

            return handlerResult;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    private async Task<Result> InvokeNewCommandAsync(
        ICliCommandHandler<TCommand, TCommandResultContract> handler,
        TCommandResultContract commandResult,
        CancellationToken token)
    {
        var aggregateResult = Result.Success;
        foreach (var args in handler.GetNewCommandsArgs(commandResult))
        {
            aggregateResult &= await _commandInvoker.TryInvokeAsync(args, token);
            if (aggregateResult.IsSuccessful is false)
                return aggregateResult;
        }

        return aggregateResult;
    }

    protected virtual void OnBuilt(
        TCommand command)
    {
    }
}
