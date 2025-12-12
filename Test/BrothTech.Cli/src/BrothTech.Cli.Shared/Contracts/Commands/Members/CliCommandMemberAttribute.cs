using BrothTech.Shared.Contracts.Results;
using System.CommandLine;

namespace BrothTech.Cli.Shared.Contracts.Commands.Members;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public abstract class CliCommandMemberAttribute :
    Attribute
{
    public abstract Result TryGetValue(
        ParseResult parseResult);
}