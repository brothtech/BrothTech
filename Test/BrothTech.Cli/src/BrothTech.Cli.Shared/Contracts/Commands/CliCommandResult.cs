namespace BrothTech.Cli.Shared.Contracts.Commands;

public interface ICliCommandResult<TCliCommand>
    where TCliCommand : class, ICliCommand, new()
{
    TCliCommand Command { get; set; }

    MutableParseResult ParseResult { get; set; }
}

public abstract class CliCommandResult<TCommand> :
    ICliCommandResult<TCommand>
    where TCommand : class, ICliCommand, new()
{
    public TCommand Command
    {
        get => field.EnsureNotNull();
        set => field = value.EnsureNotNull();
    }

    public MutableParseResult ParseResult
    {
        get => field.EnsureNotNull();
        set => field = value.EnsureNotNull();
    }
}
