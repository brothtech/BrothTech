using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands;

public interface ICliRequest<TCommand>
    where TCommand : Command, new();