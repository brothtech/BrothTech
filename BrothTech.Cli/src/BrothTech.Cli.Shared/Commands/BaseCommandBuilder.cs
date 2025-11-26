using BrothTech.Cli.Shared.Contracts;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Commands;

public abstract class BaseCommandBuilder<TParentCommand, TCommand, TCommandResult>(
    ILogger logger,
    IEnumerable<ICommandHandler<TCommand, TCommandResult>> handlers,
    IEnumerable<ICommandBuilder<TCommand>> childBuilders) :
    ICommandBuilder<TParentCommand>
    where TParentCommand : Command, new()
    where TCommand : Command, new()
    where TCommandResult : ICommandResult<TCommand>, new()
{
    private readonly ILogger _logger = logger.EnsureNotNull();
    private readonly IEnumerable<ICommandHandler<TCommand, TCommandResult>> _handlers = handlers.EnsureNotNull();
    private readonly IEnumerable<ICommandBuilder<TCommand>> _childBuilders = childBuilders.EnsureNotNull();
    private TCommand? _command;

    protected virtual bool IsRoot => false;

    public Command? Build()
    {
        if (_command is not null)
            return _command;

        try
        {
            _command = new TCommand();
            _command.SetAction(HandleAsync);
            AddChildren(_command);
            return _command;
        }
        catch (Exception exception)
        {
            if (IsRoot is false)
                throw;

            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(
                    exception: exception,
                    message: "An error occurred building command {command}",
                    GetType().Name);

            return null;
        }
    }

    private async Task HandleAsync(
        ParseResult parseResult,
        CancellationToken token)
    {
        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
        var command = _command.EnsureNotNull();
        foreach (var handler in _handlers)
            await ExecuteHandlerAsync(parseResult, tokenSource, command, handler);
    }

    private async Task ExecuteHandlerAsync(
        ParseResult parseResult,
        CancellationTokenSource tokenSource,
        TCommand command,
        ICommandHandler<TCommand, TCommandResult> handler)
    {
        try
        {
            var commandResult = new TCommandResult()
            {
                Command = command,
                ParseResult = parseResult
            };
            var result = await handler.TryHandleAsync(commandResult, tokenSource.Token);
            if (result.IsSuccessful)
                return;

            foreach (var message in result.Messages)
                _logger.Log(message.LogLevel, message.Message, message.Args);
        }
        catch (Exception exception)
        {
            tokenSource.Cancel();
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(
                    exception: exception,
                    message: "An error occurred executing {handler}",
                    handler.GetType().Name);
        }
    }

    private void AddChildren(
        Command command)
    {
        foreach (var builder in _childBuilders)
            command.Add(builder.Build().EnsureNotNull());
    }
}
