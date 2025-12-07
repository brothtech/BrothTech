using BrothTech.Cli.Shared.CliCommands;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts;

public interface ICommandResult<TCommand> :
    ICommandResult
    where TCommand : Command
{
    TCommand Command { get; set; }
}

public interface ICommandResult
{
    MutableParseResult ParseResult { get; set; }
}