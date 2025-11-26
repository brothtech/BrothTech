using BrothTech.Cli.Shared.Contracts;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Commands;

public class BaseCommandResult<TCommand> :
    ICommandResult<TCommand>
    where TCommand : Command, new()
{
    public TCommand Command
    {
        get => field.EnsureNotNull();
        set => field = value.EnsureNotNull();
    }

    public ParseResult ParseResult
    {
        get => field.EnsureNotNull();
        set => field = value.EnsureNotNull();
    }
}