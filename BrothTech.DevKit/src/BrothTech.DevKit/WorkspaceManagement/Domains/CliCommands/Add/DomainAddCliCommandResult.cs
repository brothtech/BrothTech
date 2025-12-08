using BrothTech.Cli.Shared.CliCommands;

namespace BrothTech.DevKit.WorkspaceManagement.Domains.CliCommands.Add;

public interface IDomainAddCliCommandResult :
    IBaseDomainAddCliCommandResult<DomainAddCliCommand>
{
    string? ParentDomainName
    {
        get => ParseResult.GetValue(Command.ParentDomainName);
        set => ParseResult.SetValue(Command.ParentDomainName, value.EnsureNotNull());
    }

    string[] DomainReferences
    {
        get => ParseResult.GetValue(Command.DomainReferences) ?? [];
        set => ParseResult.SetValue(Command.DomainReferences, value.EnsureNotNull());
    }
}

public class DomainAddCliCommandResult :
    CliCommandResult<DomainAddCliCommand>,
    IDomainAddCliCommandResult;