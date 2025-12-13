using BrothTech.Shared.Contracts.Results;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands;

public interface ICliCommandBuilder<TParentCommand> :
    ICliCommandBuilder
    where TParentCommand : Command, new();

public interface ICliCommandBuilder
{
    Result<Command> TryBuild();
}
