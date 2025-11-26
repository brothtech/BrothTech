using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts;

public interface ICommandResult<TCommand>
    where TCommand : Command
{
    TCommand Command { get; set; }
    ParseResult ParseResult { get; set; }
}