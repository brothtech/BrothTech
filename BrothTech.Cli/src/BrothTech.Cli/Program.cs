using BrothTech;
using BrothTech.Cli.Commands.Root;
using BrothTech.Cli.Infrastructure.DependencyInjection;
using BrothTech.DevKit.Infrastructure.DependencyInjection;
using BrothTech.Infrastructure;
using Microsoft.Extensions.Logging;

var entryPoint = new EntryPoint(
    typeof(CliServicesRegistration),
    typeof(DevKitServicesRegistration));

return await entryPoint.RunAsync<RootCommandBuilder, ILogger<Program>>(async (rootCommandBuilder, logger) => 
{
    if (rootCommandBuilder.TryBuild().OutWithNoItem(out var rootCommand, out var messages))
    {
        logger.Log(messages);
        return 1;
    }

    var result = rootCommand.Parse(args);
    return await result.InvokeAsync();
});

//TODO: test the different reference types with the project add
//TODO: recreate solution in a test directory using tools, move code over to it
//TODO: need a service to host that can live for certain commands.