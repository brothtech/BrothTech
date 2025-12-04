using BrothTech.Cli.Shared.Commands;

namespace BrothTech.Cli.Commands.Batch;

public class BatchCommandResult :
    BaseCommandResult<BatchCommand>
{
    public string[] Batches
    {
        get => field ??= ParseResult.GetRequiredValue(Command.Batches);
        set => field = value.EnsureNotNull();
    }
}
