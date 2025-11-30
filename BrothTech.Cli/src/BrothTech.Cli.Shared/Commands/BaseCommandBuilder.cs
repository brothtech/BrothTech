using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Commands;

public abstract class BaseCommandBuilder<TParentCommand, TCommand, TCommandResult>(
    ILogger logger,
    IEnumerable<ICommandBuilder<TCommand>> childBuilders,
    IEnumerable<ICommandHandler<TCommand, TCommandResult>> handlers,
    ICliCommandInvoker commandInvoker) :
    ICommandBuilder<TParentCommand>
    where TParentCommand : Command, new()
    where TCommand : Command, new()
    where TCommandResult : ICommandResult<TCommand>, new()
{
    private readonly ILogger _logger = logger.EnsureNotNull();
    private readonly IEnumerable<ICommandBuilder<TCommand>> _childBuilders = childBuilders.EnsureNotNull();
    private readonly IEnumerable<ICommandHandler<TCommand, TCommandResult>> _handlers = handlers.EnsureNotNull();
    private readonly ICliCommandInvoker _commandInvoker = commandInvoker.EnsureNotNull();
    private TCommand? _command;

    public virtual Result<Command> TryBuild()
    {
        if (_command is not null)
            return _command;

        _command = new TCommand();
        if (TryAddChildren(_command).HasFailed(out _, out var messages))
            return ErrorResult.FromMessages(messages);

        _command.SetAction(HandleAsync);
        OnBuilt(_command);
        return _command;
    }

    private Result TryAddChildren(
        Command command)
    {
        var aggregateResult = Result.Success;
        foreach (var builder in _childBuilders)
        {
            aggregateResult &= builder.TryBuild().OutWithItem(out var child);
            if (aggregateResult.IsSuccessful is false)
                return aggregateResult;

            command.Add(child);
        }

        return aggregateResult;
    }

    private async Task HandleAsync(
        ParseResult parseResult,
        CancellationToken token)
    {
        var command = _command.EnsureNotNull();
        var result = await TryHandleAsync(parseResult, command, token);
        if (result.IsSuccessful)
            return;

        foreach (var message in result.Messages)
            _logger.Log(message.LogLevel, message.Message, message.Args);
    }

    public async Task<Result> TryHandleAsync(
        ParseResult parseResult,
        TCommand command,
        CancellationToken token)
    {
        var aggregateResult = Result.Success;
        foreach (var handler in _handlers.OrderBy(x => x.Priority))
        {
            aggregateResult &= await TryExecuteHandlerAsync(parseResult, command, handler, token);
            if (aggregateResult.IsSuccessful is false)
                return aggregateResult;
        }

        return aggregateResult;
    }

    private async Task<Result> TryExecuteHandlerAsync(
        ParseResult parseResult,
        TCommand command,
        ICommandHandler<TCommand, TCommandResult> handler,
        CancellationToken token)
    {
        try
        {
            var commandResult = new TCommandResult()
            {
                Command = command,
                ParseResult = parseResult
            };
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
        ICommandHandler<TCommand, TCommandResult> handler,
        TCommandResult commandResult,
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
