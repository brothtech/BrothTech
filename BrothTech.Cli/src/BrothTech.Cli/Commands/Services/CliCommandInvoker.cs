using BrothTech.Cli.Commands.Root;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Contracts.Results;
using Microsoft.Extensions.Logging;

namespace BrothTech.Cli.Commands.Services;

public class CliCommandInvoker(
    RootCommandBuilder rootCommandBuilder,
    ILogger<CliCommandInvoker> logger) :
    ICliCommandInvoker
{
    private readonly RootCommandBuilder _rootCommandBuilder = rootCommandBuilder.EnsureNotNull();
    private readonly ILogger<CliCommandInvoker> _logger = logger.EnsureNotNull();

    public async Task<Result> TryInvokeAsync(
        string[] args,
        CancellationToken token = default)
    {
        if (_rootCommandBuilder.TryBuild().OutWithNoItem(out var rootCommand, out var messages))
        {
            _logger.Log(messages);
            return ErrorResult.FromMessages(messages);
        }

        var result = rootCommand.Parse(args);
        return await result.InvokeAsync();
    }
}
