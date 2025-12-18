using BrothTech;
using BrothTech.Cli.Infrastructure.DependencyInjection;
using BrothTech.Cli.Shared.Contracts.Commands.Root;
using BrothTech.Infrastructure;
using BrothTech.Infrastructure.DependencyInjection;
using BrothTech.WorkspaceManagement.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Logging;

var entryPoint = new EntryPoint(
    typeof(BrothTechServiceRegistration),
    typeof(CliServiceRegistration),
    typeof(WorkspaceManagementServiceRegistration));

return await entryPoint.RunAsync<IRootCliCommandBuilder, ILogger<Program>>(async (rootCommandBuilder, logger) =>
{
    if (rootCommandBuilder.TryBuild().OutWithNoItem(out var rootCommand, out var messages))
    {
        logger.Log(messages);
        return 1;
    }

    var result = rootCommand.Parse(args);
    return await result.InvokeAsync();
});