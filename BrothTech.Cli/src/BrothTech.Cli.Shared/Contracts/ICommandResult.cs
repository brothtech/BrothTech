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
    ParseResult ParseResult { get; set; }
}