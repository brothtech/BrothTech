using BrothTech;
using BrothTech.Cli.Internal.Commands.Root;
using BrothTech.Cli.Internal.Infrastructure.DependencyInjection;
using BrothTech.Infrastructure;
using Microsoft.Extensions.Logging;

var entryPoint = new EntryPoint(
    typeof(BrothTechCliServiceRegistration),
    typeof(BrothTechCliServiceRegistration));

return await entryPoint.RunAsync<RootCliCommandBuilder, ILogger<Program>>(async (rootCommandBuilder, logger) =>
{
    if (rootCommandBuilder.TryBuild().OutWithNoItem(out var rootCommand, out var messages))
    {
        logger.Log(messages);
        return 1;
    }

    var result = rootCommand.Parse(args);
    return await result.InvokeAsync();
});