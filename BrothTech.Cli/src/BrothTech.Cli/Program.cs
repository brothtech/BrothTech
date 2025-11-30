using BrothTech.Cli.Infrastructure.DependencyInjection;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.DevKit.Infrastructure.DependencyInjection;
using BrothTech.Infrastructure;

var entryPoint = new EntryPoint(
    typeof(CliServicesRegistration),
    typeof(DevKitServicesRegistration));

return await entryPoint.RunAsync<ICliCommandInvoker>(async x => 
{
    return await x.TryInvokeAsync(args);
});