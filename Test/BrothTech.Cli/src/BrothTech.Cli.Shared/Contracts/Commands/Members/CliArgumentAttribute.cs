using BrothTech.Shared.Contracts.Results;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands.Members;

public abstract class CliArgumentAttribute<T> :
    CliArgumentAttribute
{
    public sealed override Result TryGetValue(
        ParseResult parseResult)
    {
        if (parseResult.GetResult(Argument) is { } ArgumentResult)
            return Result.From(ArgumentResult.GetValueOrDefault<T>());

        return ErrorResult.FromMessages(("Argument {argumentName} is required but was not provided.", Argument.Name));
    }
}

public abstract class CliArgumentAttribute :
    CliCommandMemberAttribute
{
    public abstract Argument Argument { get; }
}
