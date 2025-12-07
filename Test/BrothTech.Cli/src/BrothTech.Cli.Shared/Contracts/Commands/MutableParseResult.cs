using System;
using System.CommandLine;

namespace BrothTech.Cli.Shared.CliCommands;

public class MutableParseResult(
    ParseResult parseResult)
{
    private readonly ParseResult _parseResult = parseResult.EnsureNotNull();
    private readonly Dictionary<object, object?> _values = [];

    public T? GetValue<T>(
        Argument<T> argument)
    {
        if (_values.TryGetValue(argument, out var value) && value?.Equals(default(T)) is false)
            return (T?)value;

        return _parseResult.GetValue(argument);
    }

    public T? GetValue<T>(
        Option<T> option)
    {
        if (_values.TryGetValue(option, out var value) && value?.Equals(default(T)) is false)
            return (T?)value;

        return _parseResult.GetValue(option);
    }

    public T? GetValue<T>(
        string name)
    {
        if (_values.TryGetValue(name, out var value) && value?.Equals(default(T)) is false)
            return (T?)value;

        return _parseResult.GetValue<T>(name);
    }

    public T GetRequiredValue<T>(
        Argument<T> argument)
    {
        if (_values.TryGetValue(argument, out var value) && value is T castValue)
            return castValue;

        return _parseResult.GetRequiredValue(argument);
    }

    public T GetRequiredValue<T>(
        Option<T> option)
    {
        if (_values.TryGetValue(option, out var value) && value is T castValue)
            return castValue;

        return _parseResult.GetRequiredValue(option);
    }

    public T GetRequiredValue<T>(
        string name)
    {
        if (_values.TryGetValue(name, out var value) && value is T castValue)
            return castValue;

        return _parseResult.GetRequiredValue<T>(name);
    }

    public void SetValue<T>(
        Argument<T> argument,
        T value)
    {
        _values[argument] = value;
    }

    public void SetValue<T>(
        Option<T> option,
        T value)
    {
        _values[option] = value;
    }

    public void SetValue<T>(
        string name,
        T value)
    {
        _values[name] = value;
    }

    public static implicit operator MutableParseResult(
        ParseResult parseResult)
    {
        return new MutableParseResult(parseResult);
    }
}