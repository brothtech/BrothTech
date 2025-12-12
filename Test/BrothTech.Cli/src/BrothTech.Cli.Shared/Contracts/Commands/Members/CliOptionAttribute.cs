using BrothTech.Shared.Contracts.Results;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands.Members;

public abstract class CliOptionAttribute<T> :
    CliOptionAttribute
{
    public sealed override Result TryGetValue(
        ParseResult parseResult)
    {
        if (parseResult.GetResult(Option) is { } optionResult)
            return Result.From(optionResult.GetValueOrDefault<T>());

        if (IsRequired is false)
            return Result.From(default(T?));

        return ErrorResult.FromMessages(("Option {optionName} is required but was not provided.", Option.Name));
    }
}

public abstract class CliOptionAttribute :
    CliCommandMemberAttribute
{
    public abstract Option Option { get; }    

    public virtual bool IsRequired => false;
}