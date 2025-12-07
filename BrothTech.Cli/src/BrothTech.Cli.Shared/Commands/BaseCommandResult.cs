using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.Contracts;
using System.CommandLine;
using System.CommandLine.Completions;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Dynamic;
using System.Xml.Linq;

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

    public MutableParseResult ParseResult
    {
        get => field.EnsureNotNull();
        set => field = value.EnsureNotNull();
    }
}

