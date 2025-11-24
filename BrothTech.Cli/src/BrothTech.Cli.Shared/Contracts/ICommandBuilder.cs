using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts;

public interface ICommandBuilder<TParentCommand> : 
    ICommandBuilder
    where TParentCommand : Command;

public interface ICommandBuilder
{
    bool IsChild(
        Command command);

    Command? Build();
}