using BrothTech;
using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Cli.Shared.Contracts.Commands.Members;
using BrothTech.Shared;
using BrothTech.Shared.Contracts.Results;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Reflection;

namespace BrothTech.Cli.Shared;

public abstract class CliCommandBuilder<TParentCommand, TCommand, TRequest>(
    ILogger logger,
    IEnumerable<ICliCommandBuilder<TCommand>> childBuilders,
    ICliRequestInvoker requestInvoker) :
    ICliCommandBuilder<TParentCommand>
    where TParentCommand : Command, new()
    where TCommand : Command, new()
    where TRequest : class, ICliRequest<TCommand>, new()
{
    private readonly ILogger _logger = logger.EnsureNotNull();
    private readonly IEnumerable<ICliCommandBuilder<TCommand>> _childBuilders = childBuilders.EnsureNotNull();
    private readonly ICliRequestInvoker _requestInvoker = requestInvoker.EnsureNotNull();
    private TCommand? _command;

    public virtual Result<Command> TryBuild()
    {
        if (_command is not null)
            return _command;

        _command = new TCommand();
        _command.SetAction(HandleAsync);
        if (TryAddCommandMembers(_command).HasFailed(out _, out var messages))
            return ErrorResult.FromMessages(messages);

        OnBuilt(_command);
        return _command;
    }

    private Result TryAddCommandMembers(
        TCommand command)
    {
        var addChildrenRresult = _childBuilders.AggregateResults(x => x.TryBuild(), command.Add);
        if (addChildrenRresult.HasFailed(out _, out var messages))
            return ErrorResult.FromMessages(messages);

        foreach (var member in typeof(TCommand).GetCustomAttributes<CliCommandMemberAttribute>(inherit: true))
            if (member is CliArgumentAttribute argumentAttribute)
                command.Add(argumentAttribute.Argument);
            else if (member is CliOptionAttribute optionAttribute)
                command.Add(optionAttribute.Option);

        return Result.Success;
    }

    protected virtual void OnBuilt(
        TCommand command)
    {
    }

    private async Task HandleAsync(
        ParseResult parseResult,
        CancellationToken token)
    {
        var result = TryBuildRequest(parseResult).OutWithItem(out var request) &&
                     await _requestInvoker.TryInvokeAsync<TCommand, TRequest>(request, token);
        if (result.IsSuccessful)
            return;

        foreach (var message in result.Messages)
            _logger.Log(message.LogLevel, message.Message, message.Args);
    }

    private Result<TRequest> TryBuildRequest(
        ParseResult parseResult)
    {
        var request = new TRequest();
        foreach (var property in typeof(TRequest).GetProperties())
        {
            if (property.GetCustomAttribute<CliCommandMemberAttribute>(inherit: true) is not { } member)
                continue;

            if (member.TryGetValue(parseResult).HasItem(out var value, out var messages) is false)
                return ErrorResult.FromMessages(messages);

            property.SetValue(request, value);
        }

        return request;
    }
}