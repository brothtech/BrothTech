namespace BrothTech.DevKit.WorkspaceManagement.Domains.Commands.Add;

public class DomainAddCommandResult :
    BaseDomainAddCommandResult<DomainAddCommand>
{
    public string? ParentDomainName => field ??= ParseResult.GetValue(Command.ParentDomainName);
}