using BrothTech.Cli.Shared.Contracts;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Commands;

public abstract class BaseCommandBuilder<TCommand>(
    ILogger logger,
    IEnumerable<ICommandHandler<TCommand>> handlers,
    IEnumerable<ICommandBuilder> builders) :
    ICommandBuilder
    where TCommand : Command
{
    private readonly ILogger _logger = logger.EnsureNotNull();
    private readonly IEnumerable<ICommandHandler<TCommand>> _handlers = handlers.EnsureNotNull();
    private readonly IEnumerable<ICommandBuilder> _builders = builders.EnsureNotNull();
    private TCommand? _command;

    protected virtual bool IsRoot => false;

    public abstract bool IsChild(
        Command command);

    public Command? Build()
    {
        if (_command is not null)
            return _command;

        try
        {
            _command = BuildInternal();
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

    protected abstract TCommand BuildInternal();

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
        ICommandHandler<TCommand> handler)
    {
        try
        {
            await handler.HandleAsync(command, parseResult, tokenSource.Token);
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
        foreach (var builder in _builders)
            if (builder.IsChild(command))
                command.Add(builder.Build().EnsureNotNull());
    }
}
