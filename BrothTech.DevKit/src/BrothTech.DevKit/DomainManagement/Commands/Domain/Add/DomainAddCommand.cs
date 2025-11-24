using System.CommandLine;

namespace BrothTech.DevKit.DomainManagement.Commands.Domain.Add;

public class DomainAddCommand :
    Command
{
    public DomainAddCommand() :
        base(nameof(DomainAddCommand))
    {
        Aliases.Add("add");
        Add(Name);
        Add(Type);
    }

    public new Argument<string> Name { get; } = new(nameof(Name).ToLower());

    public Option<DomainType> Type { get; } = new(nameof(Type).ToLower(), "t", "type");
}