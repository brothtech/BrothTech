using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands;

public interface ICliCommand
{
    Argument<T> GetOrCreateArgument<T>(
        string name);

    Option<T> GetOrCreateOption<T>(
        string name,
        params string[] aliases);

    void AddArgument(
        Argument argument);

    public void AddOption(
        Option option);

    void AddCommand(
        ICliCommand command);

    void SetHandler(
        Func<ParseResult, CancellationToken, Task> handler);

    ParseResult Parse(
        IReadOnlyList<string> args);
}

public abstract class CliCommand :
    ICliCommand
{
    private readonly Dictionary<string, object> _values = [];
    private readonly Command _command;

    protected CliCommand(
        string name,
        string? description = null) :
        this(new Command(name, description))
    {
    }

    protected CliCommand(
        Command command)
    {
        _command = command;
        foreach (var alias in GetAliases())
            _command.Aliases.Add(alias);
    }

    protected virtual IEnumerable<string> GetAliases()
    {
        return [];
    }

    public Argument<T> GetOrCreateArgument<T>(
        string name)
    {
        if (_values.TryGetValue(name, out var value))
            return value.EnsureIs<Argument<T>>();

        var argument = new Argument<T>(name);
        _values[name] = argument;
        return argument;
    }

    public Option<T> GetOrCreateOption<T>(
        string name,
        params string[] aliases)
    {
        if (_values.TryGetValue(name, out var value))
            return value.EnsureIs<Option<T>>();

        var option = new Option<T>(name, aliases);
        _values[name] = option;
        return option;
    }

    public void AddArgument(
        Argument argument)
    {
        _command.Add(argument);
    }

    public void AddOption(
        Option option)
    {
        _command.Add(option);
    }

    public void AddCommand(
        ICliCommand command)
    {
        _command.Add(command.EnsureIs<ICliCommand, CliCommand>());
    }

    public void SetHandler(
        Func<ParseResult, CancellationToken, Task> handler)
    {
        _command.SetAction(handler);
    }

    public ParseResult Parse(
        IReadOnlyList<string> args)
    {
        return _command.Parse(args);
    }

    public static implicit operator Command (
        CliCommand command)
    {
        return command._command;
    }
}