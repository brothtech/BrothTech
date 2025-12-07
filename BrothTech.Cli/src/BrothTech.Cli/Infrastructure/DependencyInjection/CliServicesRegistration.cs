using BrothTech.Cli.Commands.Services;
using BrothTech.Cli.Shared.CliCommands;
using BrothTech.Cli.Shared.Contracts;
using BrothTech.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace BrothTech.Cli.Infrastructure.DependencyInjection;

public class CliServicesRegistration :
    DomainServicesRegistration
{
    protected override Type MarkerType => typeof(CliMarker);

    protected override bool ShouldRegisterScannedService(
        Type type)
    {
        return type.IsAssignableTo(typeof(ICliCommandHandler)) ||
               type.IsAssignableTo(typeof(ICliCommandBuilder));
    }

    protected override void RegisterAdditionalServices(
        IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ICliCommandInvoker, CliCommandInvoker>();
    }
}
