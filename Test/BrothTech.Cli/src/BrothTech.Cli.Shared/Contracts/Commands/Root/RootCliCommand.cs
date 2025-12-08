using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands.Root;

public class RootCliCommand() : 
    CliCommand(new RootCommand());
