using BrothTech.Cli.Internal.Commands;
using BrothTech.Cli.Shared;
using BrothTech.Cli.Shared.Contracts.Commands;
using BrothTech.Shared.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace BrothTech.Cli.Internal.Infrastructure.DependencyInjection;

public class BrothTechCliServiceRegistration :
    DomainServicesRegistration
{
    protected override Type MarkerType => typeof(BrothTechCliInternalMarker);

    protected override bool ShouldRegisterScannedService(
        Type type)
    {
        return type.IsAssignableTo(typeof(ICliCommandBuilder)) ||
               type.IsAssignableTo(typeof(ICliRequestHandler)) ||
               type.IsAssignableTo(typeof(ICliRequestValidator));
    }

    protected override void RegisterAdditionalServices(
        IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ICliCommandInvoker, CliCommandInvoker>();
        serviceCollection.AddSingleton<ICliRequestInvoker, CliRequestInvoker>();
    }
}
