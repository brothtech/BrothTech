using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts;

public interface ICommandBuilder<TParentCommand> : 
    ICommandBuilder
    where TParentCommand : Command, new();

public interface ICommandBuilder
{
    Command? Build();
}