namespace BrothTech.Cli.Shared.CliCommands.Batch;

public class BatchCliCommandResult :
    CliCommandResult<BatchCliCommand>
{
    public string? BatchSourcePath
    {
        get => ParseResult.GetValue(Command.BatchSourcePath);
        set => ParseResult.SetValue(Command.BatchSourcePath, value.EnsureNotNull());
    }

    public string[] Batches
    {
        get => ParseResult.GetValue(Command.Batches) ?? [];
        set => ParseResult.SetValue(Command.Batches, value.EnsureNotNull());
    }
}
