using BrothTech;
using BrothTech.Cli;
using BrothTech.Cli.Infrastructure.DependencyInjection;
using BrothTech.DevKit.Infrastructure.DependencyInjection;
using BrothTech.Infrastructure;

var entryPoint = new EntryPoint(
    typeof(CliServicesRegistration),
    typeof(DevKitServicesRegistration));

return await entryPoint.RunAsync<RootCommandBuilder>(async x =>
{
    var rootCommand = x.Build().EnsureNotNull();
    var result = rootCommand.Parse(args);
    return await result.InvokeAsync(); 
});