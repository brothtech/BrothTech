using BrothTech.Shared;
using BrothTech.Shared.Contracts.Results;
using System.Collections.Concurrent;
using System.CommandLine;
using System.Reflection;

namespace BrothTech.Cli.Shared.Contracts.Commands;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public abstract class CliCommandMembersAttribute :
    Attribute
{
    private readonly ConcurrentDictionary<Type, ValueExtractor> _extractors = [];

    protected abstract Dictionary<string, Symbol> Members { get; }

    public void AddCommandMembers(
        Command command)
    {
        foreach (var argument in Members.Values.OfType<Argument>())
            command.Add(argument);

        foreach (var option in Members.Values.OfType<Command>())
            command.Add(option);
    }

    public Result TryBindMemberValues(
        ParseResult parseResult,
        object target)
    {
        return target.GetType()
                     .GetProperties()
                     .AggregateResults(x => TryBindMemberValue(parseResult, target, x));
    }

    private Result TryBindMemberValue(
        ParseResult parseResult,
        object target,
        PropertyInfo property)
    {
        try
        {
            if (Members.TryGetValue(property.Name, out var member) is false)
                return Result.Success;

            if (DoesMemberAndTargetPropertyTypeMatch(property, member) is false)
                return ErrorResult.FromMessages(("Command and Target member mismath on {name}.", property.Name));

            var valueExtractor = GetValueExtractor(property.PropertyType);
            if (valueExtractor.TryGetValueOrDefault(parseResult, member).HasItem(out var value, out var messages) is false)
                return ErrorResult.FromMessages(messages);

            property.SetValue(target, value);
            return Result.Success;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }
    
    private bool DoesMemberAndTargetPropertyTypeMatch(
        PropertyInfo property,
        Symbol member)
    {
        var type = member.GetType();
        return type.IsGenericType &&
               type.GenericTypeArguments.Length == 1 &&
               type.GenericTypeArguments[0].IsAssignableTo(property.PropertyType);
    }

    private ValueExtractor GetValueExtractor(
        Type type)
    {
        return _extractors.GetOrAdd(
            type,
            valueFactory: x => (ValueExtractor)Activator.CreateInstance(typeof(ValueExtractor<>).MakeGenericType(type))!);
    }

    private class ValueExtractor<T> :
        ValueExtractor
    {
        public override Result TryGetValueOrDefault(
            ParseResult parseResult, 
            Symbol member)
        {
            return member switch
            {
                Argument argument => TryGetArgumentValueOrDefault(parseResult, argument),
                Option option => TryGetOptionValueOrDefault(parseResult, option),
                _ => ErrorResult.FromErrorMessages("Command member was not an argument or option.")
            };
        }

        private Result TryGetArgumentValueOrDefault(
            ParseResult parseResult,
            Argument argument)
        {
            if (parseResult.GetResult(argument) is { } argumentResult)
                return Result.From(argumentResult.GetValueOrDefault<T>());

            return ErrorResult.FromMessages(("Argument {argumentName} is required but was not provided.", argument.Name));
        }

        private Result TryGetOptionValueOrDefault(
            ParseResult parseResult,
            Option option)
        {
            if (parseResult.GetResult(option) is { } optionResult)
                return Result.From(optionResult.GetValueOrDefault<T>());

            if (option.Required is false)
                return Result.From(default(T?));

            return ErrorResult.FromMessages(("Option {optionName} is required but was not provided.", option.Name));
        }
    }

    private abstract class ValueExtractor
    {
        public abstract Result TryGetValueOrDefault(
            ParseResult parseResult,
            Symbol member);
    }
}
